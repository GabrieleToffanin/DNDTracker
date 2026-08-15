import { useState } from 'react';
import type { MonsterStatBlock } from '../api/types';
import { addMonster } from '../api/api';

interface Props { campaignName: string; monsters: MonsterStatBlock[]; onRefresh: () => void; }

export default function MonstersPanel({ campaignName, monsters, onRefresh }: Props) {
  const [show, setShow] = useState(false);
  const [selected, setSelected] = useState<MonsterStatBlock | null>(null);

  return (
    <div>
      <div className="panel-header">
        <h3>👾 Monster Library ({monsters.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShow(true)}>+ Add Monster</button>
      </div>
      {monsters.length === 0 && <p className="text-dim">No monsters in library yet.</p>}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: '.6rem' }}>
        {monsters.map(m => (
          <div key={m.id} className="panel" style={{ cursor: 'pointer' }} onClick={() => setSelected(m)}>
            <div style={{ fontFamily: 'var(--font-heading)', color: 'var(--clr-red-lt)', marginBottom: '.2rem' }}>{m.name}</div>
            <div className="text-dim" style={{ fontSize: '.8rem' }}>{m.creatureType}</div>
            <div className="flex gap-sm" style={{ marginTop: '.4rem', flexWrap: 'wrap' }}>
              <span className="badge badge-red">HP {m.hitPoints}</span>
              <span className="badge badge-dim">AC {m.armorClass}</span>
              <span className="badge badge-gold">CR {m.challengeRating}</span>
            </div>
          </div>
        ))}
      </div>
      {show && <AddMonsterModal campaignName={campaignName} onClose={() => setShow(false)} onCreated={() => { setShow(false); onRefresh(); }} />}
      {selected && (
        <div className="modal-backdrop" onClick={() => setSelected(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>👾 {selected.name}</h2>
            <p className="text-dim" style={{ marginBottom: '.75rem' }}>{selected.creatureType}</p>
            <div className="flex gap-sm" style={{ flexWrap: 'wrap', marginBottom: '.5rem' }}>
              <span className="badge badge-red">HP {selected.hitPoints}</span>
              <span className="badge badge-dim">AC {selected.armorClass}</span>
              <span className="badge badge-gold">CR {selected.challengeRating}</span>
              <span className="badge badge-dim">XP {selected.experiencePoints}</span>
              <span className="badge badge-dim">Speed {selected.speed}ft</span>
              <span className="badge badge-dim">Init {selected.initiativeModifier >= 0 ? '+' : ''}{selected.initiativeModifier}</span>
            </div>
            {selected.notes && <p>{selected.notes}</p>}
            <div className="modal-actions"><button className="btn" onClick={() => setSelected(null)}>Close</button></div>
          </div>
        </div>
      )}
    </div>
  );
}

function AddMonsterModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const [f, setF] = useState({ name: '', creatureType: 'Beast', armorClass: 12, hitPoints: 20, challengeRating: 1, experiencePoints: 200, initiativeModifier: 0, speed: 30, notes: '' });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);
  const s = (k: string, v: string | number) => setF(p => ({ ...p, [k]: v }));

  const submit = async () => {
    if (!f.name.trim()) { setErr('Name required.'); return; }
    setSaving(true);
    try { await addMonster(campaignName, f); onCreated(); }
    catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>👾 Add Monster</h2>
        <div className="form-grid form-grid-2">
          <div className="form-row"><label>Name *</label><input value={f.name} onChange={e => s('name', e.target.value)} /></div>
          <div className="form-row"><label>Creature Type</label><input value={f.creatureType} onChange={e => s('creatureType', e.target.value)} /></div>
        </div>
        <div className="form-grid form-grid-3">
          <div className="form-row"><label>HP</label><input type="number" min={1} value={f.hitPoints} onChange={e => s('hitPoints', +e.target.value)} /></div>
          <div className="form-row"><label>AC</label><input type="number" min={1} value={f.armorClass} onChange={e => s('armorClass', +e.target.value)} /></div>
          <div className="form-row"><label>CR</label><input type="number" min={0} value={f.challengeRating} onChange={e => s('challengeRating', +e.target.value)} /></div>
          <div className="form-row"><label>XP</label><input type="number" min={0} value={f.experiencePoints} onChange={e => s('experiencePoints', +e.target.value)} /></div>
          <div className="form-row"><label>Init Mod</label><input type="number" value={f.initiativeModifier} onChange={e => s('initiativeModifier', +e.target.value)} /></div>
          <div className="form-row"><label>Speed (ft)</label><input type="number" min={0} value={f.speed} onChange={e => s('speed', +e.target.value)} /></div>
        </div>
        <div className="form-row"><label>Notes</label><textarea rows={2} value={f.notes} onChange={e => s('notes', e.target.value)} /></div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add Monster'}</button>
        </div>
      </div>
    </div>
  );
}
