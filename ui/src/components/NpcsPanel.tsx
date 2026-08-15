import { useState } from 'react';
import type { NpcResource } from '../api/types';
import { addNpc } from '../api/api';

interface Props { campaignName: string; npcs: NpcResource[]; onRefresh: () => void; }

export default function NpcsPanel({ campaignName, npcs, onRefresh }: Props) {
  const [show, setShow] = useState(false);

  return (
    <div>
      <div className="panel-header">
        <h3>🧙 NPCs ({npcs.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShow(true)}>+ Add NPC</button>
      </div>
      {npcs.length === 0 && <p className="text-dim">No NPCs yet.</p>}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: '.6rem' }}>
        {npcs.map(n => (
          <div key={n.id} className="panel">
            <div style={{ fontFamily: 'var(--font-heading)', color: 'var(--clr-gold-lt)', marginBottom: '.2rem' }}>{n.name}</div>
            <div className="text-dim" style={{ fontSize: '.8rem', marginBottom: '.3rem' }}>{n.role}</div>
            {n.notes && <p style={{ fontSize: '.85rem' }}>{n.notes}</p>}
          </div>
        ))}
      </div>
      {show && (
        <AddNpcModal campaignName={campaignName} onClose={() => setShow(false)} onCreated={() => { setShow(false); onRefresh(); }} />
      )}
    </div>
  );
}

function AddNpcModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const [f, setF] = useState({ name: '', role: '', notes: '' });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);
  const s = (k: string, v: string) => setF(p => ({ ...p, [k]: v }));

  const submit = async () => {
    if (!f.name.trim()) { setErr('Name required.'); return; }
    setSaving(true);
    try { await addNpc(campaignName, f); onCreated(); }
    catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>🧙 Add NPC</h2>
        <div className="form-row"><label>Name *</label><input value={f.name} onChange={e => s('name', e.target.value)} /></div>
        <div className="form-row"><label>Role</label><input value={f.role} onChange={e => s('role', e.target.value)} placeholder="e.g. Innkeeper, Villain…" /></div>
        <div className="form-row"><label>Notes</label><textarea rows={3} value={f.notes} onChange={e => s('notes', e.target.value)} /></div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add NPC'}</button>
        </div>
      </div>
    </div>
  );
}
