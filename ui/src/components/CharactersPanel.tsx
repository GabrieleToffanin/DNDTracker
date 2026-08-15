import { useState } from 'react';
import type { CharacterSheetDto } from '../api/types';
import { addHero, updateCharacterHp, addCharacterCondition } from '../api/api';
import HpBar from './HpBar';
import styles from './CharactersPanel.module.css';

const CLASSES = ['Barbarian','Bard','Cleric','Druid','Fighter','Monk','Paladin','Ranger','Rogue','Sorcerer','Warlock','Wizard'];
const RACES = ['Human','Orc','HalfOrc','HalfElf','Elf','Thiefling','Aasimar'];
const DICE = ['D4','D6','D8','D10','D12','D20','D100'];

interface Props {
  campaignName: string;
  characters: CharacterSheetDto[];
  onRefresh: () => void;
}

export default function CharactersPanel({ campaignName, characters, onRefresh }: Props) {
  const [showAdd, setShowAdd] = useState(false);
  const [selected, setSelected] = useState<CharacterSheetDto | null>(null);

  return (
    <div>
      <div className="panel-header">
        <h3>🧝 Characters ({characters.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShowAdd(true)}>+ Add Hero</button>
      </div>

      <div className={styles.grid}>
        {characters.map(c => (
          <div key={c.id} className={styles.card} onClick={() => setSelected(c)}>
            <div className={styles.cardTop}>
              <div>
                <div className={styles.name}>{c.name}</div>
                <div className="text-dim" style={{ fontSize: '.8rem' }}>{c.race} {c.class} — Lv.{c.level}</div>
              </div>
              <div className={styles.ac}>
                <span>🛡</span>
                <span>{c.armorClass}</span>
              </div>
            </div>
            <HpBar current={c.currentHitPoints} max={c.maxHitPoints} temporary={c.temporaryHitPoints} />
            {c.conditions.length > 0 && (
              <div className={styles.conditions}>
                {c.conditions.map((cond, i) => (
                  <span key={i} className="badge badge-red">{cond.name}{cond.remainingRounds != null ? ` (${cond.remainingRounds}r)` : ''}</span>
                ))}
              </div>
            )}
          </div>
        ))}
        {characters.length === 0 && <p className="text-dim">No characters yet.</p>}
      </div>

      {showAdd && (
        <AddHeroModal
          campaignName={campaignName}
          onClose={() => setShowAdd(false)}
          onCreated={() => { setShowAdd(false); onRefresh(); }}
        />
      )}
      {selected && (
        <CharacterDetailModal
          campaignName={campaignName}
          character={selected}
          onClose={() => setSelected(null)}
          onRefresh={() => { setSelected(null); onRefresh(); }}
        />
      )}
    </div>
  );
}

function CharacterDetailModal({ campaignName, character: c, onClose, onRefresh }: {
  campaignName: string; character: CharacterSheetDto; onClose: () => void; onRefresh: () => void;
}) {
  const [dmg, setDmg] = useState('');
  const [heal, setHeal] = useState('');
  const [tmp, setTmp] = useState('');
  const [cond, setCond] = useState('');
  const [condRounds, setCondRounds] = useState('');
  const [err, setErr] = useState('');

  const applyHp = async () => {
    try {
      await updateCharacterHp(campaignName, c.id, {
        damage: parseInt(dmg) || 0,
        healing: parseInt(heal) || 0,
        temporaryHitPointsDelta: parseInt(tmp) || 0,
      });
      onRefresh();
    } catch { setErr('Failed to update HP.'); }
  };

  const applyCond = async () => {
    if (!cond.trim()) return;
    try {
      await addCharacterCondition(campaignName, c.id, cond, condRounds ? parseInt(condRounds) : undefined);
      onRefresh();
    } catch { setErr('Failed to add condition.'); }
  };

  const modifier = (score: number) => {
    const m = Math.floor((score - 10) / 2);
    return m >= 0 ? `+${m}` : `${m}`;
  };

  return (
    <div className="modal-backdrop">
      <div className="modal" style={{ maxWidth: 640 }}>
        <div className="flex justify-between items-center mb-md">
          <h2>{c.name}</h2>
          <button className="btn btn-sm" onClick={onClose}>✕</button>
        </div>
        <div className="text-dim mb-md">{c.race} · {c.class} · Level {c.level} · {c.background}</div>

        <div className={styles.abilityRow}>
          {(['strength','dexterity','constitution','intelligence','wisdom','charisma'] as const).map(attr => (
            <div key={attr} className={styles.abilityBox}>
              <div className={styles.abilityLabel}>{attr.slice(0,3).toUpperCase()}</div>
              <div className={styles.abilityScore}>{c.abilityScores[attr]}</div>
              <div className={styles.abilityMod}>{modifier(c.abilityScores[attr])}</div>
            </div>
          ))}
        </div>

        <div className={styles.statsRow}>
          <div className={styles.stat}><span className="text-dim">HP</span><HpBar current={c.currentHitPoints} max={c.maxHitPoints} temporary={c.temporaryHitPoints} /></div>
          <div className={styles.stat}><span className="text-dim">AC</span><strong>{c.armorClass}</strong></div>
          <div className={styles.stat}><span className="text-dim">Init</span><strong>{c.initiative >= 0 ? '+' : ''}{c.initiative}</strong></div>
          <div className={styles.stat}><span className="text-dim">Speed</span><strong>{c.speed}ft</strong></div>
        </div>

        <div className="ornament">── HP Management ──</div>
        <div className="flex gap-sm mb-sm" style={{ flexWrap: 'wrap' }}>
          <input placeholder="Damage" type="number" min={0} value={dmg} onChange={e => setDmg(e.target.value)} style={{ width: 90 }} />
          <input placeholder="Healing" type="number" min={0} value={heal} onChange={e => setHeal(e.target.value)} style={{ width: 90 }} />
          <input placeholder="Tmp HP Δ" type="number" value={tmp} onChange={e => setTmp(e.target.value)} style={{ width: 90 }} />
          <button className="btn btn-sm btn-primary" onClick={applyHp}>Apply HP</button>
        </div>

        <div className="ornament">── Add Condition ──</div>
        <div className="flex gap-sm mb-sm">
          <input placeholder="Condition (e.g. Poisoned)" value={cond} onChange={e => setCond(e.target.value)} />
          <input placeholder="Rounds" type="number" min={1} value={condRounds} onChange={e => setCondRounds(e.target.value)} style={{ width: 80 }} />
          <button className="btn btn-sm" onClick={applyCond}>Add</button>
        </div>

        {c.conditions.length > 0 && (
          <div className="flex gap-sm mb-sm" style={{ flexWrap: 'wrap' }}>
            {c.conditions.map((cd, i) => <span key={i} className="badge badge-red">{cd.name}{cd.remainingRounds != null ? ` (${cd.remainingRounds}r)` : ''}</span>)}
          </div>
        )}

        {c.notes && <p className="text-dim" style={{ marginTop: '.5rem', fontSize: '.85rem' }}><em>{c.notes}</em></p>}
        {err && <p className="error-msg">{err}</p>}
      </div>
    </div>
  );
}

function AddHeroModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const [f, setF] = useState({
    name: '', class: 'Fighter', race: 'Human', alignment: 6, level: 1, experience: 0,
    hitPoints: 10, hitDice: 'D8', armorClass: 10, initiative: 0, speed: 30,
    background: '', notes: '', isNonPlayerCharacter: false,
    str: 10, dex: 10, con: 10, int: 10, wis: 10, cha: 10,
  });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);

  const s = (k: string, v: string | number | boolean) => setF(prev => ({ ...prev, [k]: v }));

  const submit = async () => {
    if (!f.name.trim()) { setErr('Name required.'); return; }
    setSaving(true);
    try {
      await addHero(campaignName, {
        name: f.name, class: f.class as any, race: f.race as any, alignment: f.alignment,
        level: f.level, experience: f.experience, hitPoints: f.hitPoints, hitDice: f.hitDice as any,
        armorClass: f.armorClass, initiative: f.initiative, speed: f.speed,
        background: f.background, notes: f.notes, isNonPlayerCharacter: f.isNonPlayerCharacter,
        abilityScores: { strength: f.str, dexterity: f.dex, constitution: f.con, intelligence: f.int, wisdom: f.wis, charisma: f.cha },
      } as any);
      onCreated();
    } catch (e: any) {
      setErr(e?.response?.data?.detail ?? 'Failed.');
    } finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>🧝 Add Hero</h2>
        <div className="form-row"><label>Name *</label><input value={f.name} onChange={e => s('name', e.target.value)} /></div>
        <div className="form-grid form-grid-2">
          <div className="form-row"><label>Class</label>
            <select value={f.class} onChange={e => s('class', e.target.value)}>
              {CLASSES.map(c => <option key={c}>{c}</option>)}
            </select>
          </div>
          <div className="form-row"><label>Race</label>
            <select value={f.race} onChange={e => s('race', e.target.value)}>
              {RACES.map(r => <option key={r}>{r}</option>)}
            </select>
          </div>
        </div>
        <div className="form-grid form-grid-3">
          <div className="form-row"><label>Level</label><input type="number" min={1} max={20} value={f.level} onChange={e => s('level', +e.target.value)} /></div>
          <div className="form-row"><label>XP</label><input type="number" min={0} value={f.experience} onChange={e => s('experience', +e.target.value)} /></div>
          <div className="form-row"><label>HP</label><input type="number" min={1} value={f.hitPoints} onChange={e => s('hitPoints', +e.target.value)} /></div>
          <div className="form-row"><label>Hit Dice</label>
            <select value={f.hitDice} onChange={e => s('hitDice', e.target.value)}>
              {DICE.map(d => <option key={d}>{d}</option>)}
            </select>
          </div>
          <div className="form-row"><label>AC</label><input type="number" min={1} value={f.armorClass} onChange={e => s('armorClass', +e.target.value)} /></div>
          <div className="form-row"><label>Initiative</label><input type="number" value={f.initiative} onChange={e => s('initiative', +e.target.value)} /></div>
          <div className="form-row"><label>Speed (ft)</label><input type="number" min={0} value={f.speed} onChange={e => s('speed', +e.target.value)} /></div>
        </div>
        <div className="ornament">── Ability Scores ──</div>
        <div className="form-grid form-grid-3">
          {(['str','dex','con','int','wis','cha'] as const).map(attr => (
            <div key={attr} className="form-row">
              <label>{attr.toUpperCase()}</label>
              <input type="number" min={1} max={30} value={(f as any)[attr]} onChange={e => s(attr, +e.target.value)} />
            </div>
          ))}
        </div>
        <div className="form-row"><label>Background</label><input value={f.background} onChange={e => s('background', e.target.value)} /></div>
        <div className="form-row"><label>Notes</label><textarea rows={2} value={f.notes} onChange={e => s('notes', e.target.value)} /></div>
        <div className="form-row flex items-center gap-sm">
          <input type="checkbox" id="npc" checked={f.isNonPlayerCharacter} onChange={e => s('isNonPlayerCharacter', e.target.checked)} style={{ width: 'auto' }} />
          <label htmlFor="npc" style={{ marginBottom: 0 }}>NPC</label>
        </div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add Hero'}</button>
        </div>
      </div>
    </div>
  );
}
