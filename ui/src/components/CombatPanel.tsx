import { useState } from 'react';
import type { CampaignTrackerDto, CombatantState } from '../api/types';
import { startCombat, advanceCombatTurn, updateCombatHp, addCombatCondition } from '../api/api';
import HpBar from './HpBar';
import styles from './CombatPanel.module.css';

interface Props {
  campaignName: string;
  tracker: CampaignTrackerDto;
  onRefresh: () => void;
}

export default function CombatPanel({ campaignName, tracker, onRefresh }: Props) {
  const combat = tracker.activeCombat;
  const [showStart, setShowStart] = useState(false);
  const [err, setErr] = useState('');
  const [actionId, setActionId] = useState<string | null>(null);

  const advance = async () => {
    try { await advanceCombatTurn(campaignName); onRefresh(); }
    catch { setErr('Failed to advance turn.'); }
  };

  if (!combat) {
    return (
      <div>
        <div className="panel-header">
          <h3>⚔ Combat Tracker</h3>
          <button className="btn btn-sm btn-primary" onClick={() => setShowStart(true)}>⚔ Start Combat</button>
        </div>
        <p className="text-dim">No active combat. Start a new encounter!</p>
        {showStart && (
          <StartCombatModal
            campaignName={campaignName}
            tracker={tracker}
            onClose={() => setShowStart(false)}
            onStarted={() => { setShowStart(false); onRefresh(); }}
          />
        )}
      </div>
    );
  }

  const current = combat.initiativeOrder[combat.turnIndex];

  return (
    <div>
      <div className="panel-header">
        <h3>⚔ Combat — Round {combat.round}</h3>
        <div className="flex gap-sm">
          <button className="btn btn-sm" onClick={advance}>Next Turn ▶</button>
          <button className="btn btn-sm" onClick={onRefresh}>↻</button>
        </div>
      </div>

      {current && (
        <div className={styles.turnBanner}>
          <span className="text-dim" style={{ fontSize: '.8rem' }}>Active:</span>
          <span className={styles.turnName}>{current.name}</span>
          <span className="badge badge-gold">{current.type}</span>
        </div>
      )}

      {err && <p className="error-msg">{err}</p>}

      <div className={styles.list}>
        {combat.initiativeOrder.map((c, i) => (
          <div key={c.id} className={`${styles.row} ${i === combat.turnIndex ? styles.active : ''}`}>
            <div className={styles.init}>{c.initiative}</div>
            <div className={styles.info}>
              <div className={styles.cname}>{c.name}</div>
              <span className={`badge ${c.type === 'Monster' ? 'badge-red' : 'badge-green'}`}>{c.type}</span>
              {c.conditions.length > 0 && c.conditions.map((cd, ci) => (
                <span key={ci} className="badge badge-dim">{cd.name}</span>
              ))}
            </div>
            <div style={{ flex: 1 }}>
              <HpBar current={c.currentHitPoints} max={c.maxHitPoints} temporary={c.temporaryHitPoints} />
            </div>
            <button className="btn btn-sm btn-icon" onClick={() => setActionId(actionId === c.id ? null : c.id)}>⚙</button>
          </div>
        ))}
      </div>

      {actionId && (() => {
        const combatant = combat.initiativeOrder.find(c => c.id === actionId);
        if (!combatant) return null;
        return (
          <CombatantActions
            campaignName={campaignName}
            combatant={combatant}
            onDone={() => { setActionId(null); onRefresh(); }}
          />
        );
      })()}
    </div>
  );
}

function CombatantActions({ campaignName, combatant, onDone }: { campaignName: string; combatant: CombatantState; onDone: () => void }) {
  const [dmg, setDmg] = useState('');
  const [heal, setHeal] = useState('');
  const [tmp, setTmp] = useState('');
  const [cond, setCond] = useState('');
  const [condR, setCondR] = useState('');
  const [err, setErr] = useState('');

  const applyHp = async () => {
    try {
      await updateCombatHp(campaignName, { combatantId: combatant.id, damage: parseInt(dmg)||0, healing: parseInt(heal)||0, temporaryHitPointsDelta: parseInt(tmp)||0 });
      onDone();
    } catch { setErr('Failed.'); }
  };

  const applyCond = async () => {
    if (!cond.trim()) return;
    try {
      await addCombatCondition(campaignName, combatant.id, cond, condR ? parseInt(condR) : undefined);
      onDone();
    } catch { setErr('Failed.'); }
  };

  return (
    <div className={styles.actions}>
      <strong>{combatant.name}</strong>
      <div className="flex gap-sm" style={{ flexWrap: 'wrap', marginTop: '.5rem' }}>
        <input placeholder="Damage" type="number" min={0} value={dmg} onChange={e => setDmg(e.target.value)} style={{ width: 80 }} />
        <input placeholder="Heal" type="number" min={0} value={heal} onChange={e => setHeal(e.target.value)} style={{ width: 80 }} />
        <input placeholder="Tmp HP" type="number" value={tmp} onChange={e => setTmp(e.target.value)} style={{ width: 80 }} />
        <button className="btn btn-sm btn-primary" onClick={applyHp}>Apply HP</button>
      </div>
      <div className="flex gap-sm" style={{ marginTop: '.4rem', flexWrap: 'wrap' }}>
        <input placeholder="Condition" value={cond} onChange={e => setCond(e.target.value)} />
        <input placeholder="Rounds" type="number" min={1} value={condR} onChange={e => setCondR(e.target.value)} style={{ width: 70 }} />
        <button className="btn btn-sm" onClick={applyCond}>Add Condition</button>
      </div>
      {err && <p className="error-msg">{err}</p>}
    </div>
  );
}

function StartCombatModal({ campaignName, tracker, onClose, onStarted }: {
  campaignName: string; tracker: CampaignTrackerDto; onClose: () => void; onStarted: () => void;
}) {
  type Row = { id: string; name: string; type: string; initiative: number; currentHitPoints: number; maxHitPoints: number; temporaryHitPoints: number; hideHitPointsFromPlayers: boolean };
  const defaultRows: Row[] = tracker.characters.map(c => ({
    id: c.id, name: c.name, type: 'Character', initiative: c.initiative,
    currentHitPoints: c.currentHitPoints, maxHitPoints: c.maxHitPoints, temporaryHitPoints: c.temporaryHitPoints, hideHitPointsFromPlayers: false,
  }));
  const [rows, setRows] = useState<Row[]>(defaultRows);
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);

  const update = (id: string, k: keyof Row, v: string | number | boolean) =>
    setRows(prev => prev.map(r => r.id === id ? { ...r, [k]: v } : r));

  const addMonster = (m: typeof tracker.monsterLibrary[number]) => {
    setRows(prev => [...prev, {
      id: crypto.randomUUID(), name: m.name, type: 'Monster',
      initiative: m.initiativeModifier, currentHitPoints: m.hitPoints, maxHitPoints: m.hitPoints,
      temporaryHitPoints: 0, hideHitPointsFromPlayers: true,
    }]);
  };

  const submit = async () => {
    setSaving(true);
    try {
      await startCombat(campaignName, rows.map(r => ({ ...r, conditions: [] })));
      onStarted();
    } catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal" style={{ maxWidth: 720 }}>
        <h2>⚔ Start Combat</h2>
        <div className={styles.startGrid}>
          <div>
            <p className="text-dim mb-sm">Combatants</p>
            {rows.map(r => (
              <div key={r.id} className={styles.combRow}>
                <input value={r.name} onChange={e => update(r.id, 'name', e.target.value)} style={{ flex: 1 }} placeholder="Name" />
                <select value={r.type} onChange={e => update(r.id, 'type', e.target.value)} style={{ width: 100 }}>
                  <option>Character</option><option>Monster</option>
                </select>
                <input type="number" value={r.initiative} onChange={e => update(r.id, 'initiative', +e.target.value)} style={{ width: 55 }} placeholder="Init" />
                <input type="number" value={r.currentHitPoints} onChange={e => update(r.id, 'currentHitPoints', +e.target.value)} style={{ width: 60 }} placeholder="HP" />
                <input type="number" value={r.maxHitPoints} onChange={e => update(r.id, 'maxHitPoints', +e.target.value)} style={{ width: 60 }} placeholder="Max" />
                <input type="checkbox" checked={r.hideHitPointsFromPlayers} onChange={e => update(r.id, 'hideHitPointsFromPlayers', e.target.checked)} style={{ width: 'auto' }} title="Hide HP from players" />
                <button className="btn btn-sm btn-icon" onClick={() => setRows(prev => prev.filter(x => x.id !== r.id))}>✕</button>
              </div>
            ))}
            <button className="btn btn-sm" style={{ marginTop: '.4rem' }} onClick={() => setRows(prev => [...prev, { id: crypto.randomUUID(), name: '', type: 'Monster', initiative: 0, currentHitPoints: 10, maxHitPoints: 10, temporaryHitPoints: 0, hideHitPointsFromPlayers: false }])}>+ Add Row</button>
          </div>
          {tracker.monsterLibrary.length > 0 && (
            <div>
              <p className="text-dim mb-sm">Monster Library</p>
              {tracker.monsterLibrary.map(m => (
                <div key={m.id} className={styles.monsterRow}>
                  <span>{m.name}</span>
                  <span className="text-dim" style={{ fontSize: '.75rem' }}>HP:{m.hitPoints} CR:{m.challengeRating}</span>
                  <button className="btn btn-sm btn-icon" onClick={() => addMonster(m)}>+</button>
                </div>
              ))}
            </div>
          )}
        </div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Starting…' : 'Start Combat'}</button>
        </div>
      </div>
    </div>
  );
}
