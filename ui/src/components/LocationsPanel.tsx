import { useState } from 'react';
import type { LocationResource } from '../api/types';
import { addLocation } from '../api/api';

interface Props { campaignName: string; locations: LocationResource[]; onRefresh: () => void; }

export default function LocationsPanel({ campaignName, locations, onRefresh }: Props) {
  const [show, setShow] = useState(false);
  const [selected, setSelected] = useState<LocationResource | null>(null);

  return (
    <div>
      <div className="panel-header">
        <h3>🗺 Locations ({locations.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShow(true)}>+ Add Location</button>
      </div>
      {locations.length === 0 && <p className="text-dim">No locations yet.</p>}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))', gap: '.6rem' }}>
        {locations.map(l => (
          <div key={l.id} className="panel" style={{ cursor: 'pointer' }} onClick={() => setSelected(l)}>
            {l.mapUrl && <img src={l.mapUrl} alt={l.name} style={{ width: '100%', height: 100, objectFit: 'cover', borderRadius: 'var(--radius)', marginBottom: '.4rem' }} />}
            <div style={{ fontFamily: 'var(--font-heading)', color: 'var(--clr-gold-lt)', marginBottom: '.2rem' }}>{l.name}</div>
            <p className="text-dim" style={{ fontSize: '.85rem' }}>{l.description.slice(0, 80)}{l.description.length > 80 ? '…' : ''}</p>
          </div>
        ))}
      </div>
      {show && <AddLocationModal campaignName={campaignName} onClose={() => setShow(false)} onCreated={() => { setShow(false); onRefresh(); }} />}
      {selected && (
        <div className="modal-backdrop" onClick={() => setSelected(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <h2>🗺 {selected.name}</h2>
            {selected.mapUrl && <img src={selected.mapUrl} alt={selected.name} style={{ width: '100%', borderRadius: 'var(--radius)', marginBottom: '.75rem' }} />}
            <p style={{ whiteSpace: 'pre-wrap' }}>{selected.description}</p>
            <div className="modal-actions"><button className="btn" onClick={() => setSelected(null)}>Close</button></div>
          </div>
        </div>
      )}
    </div>
  );
}

function AddLocationModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const [f, setF] = useState({ name: '', description: '', mapUrl: '' });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);
  const s = (k: string, v: string) => setF(p => ({ ...p, [k]: v }));

  const submit = async () => {
    if (!f.name.trim()) { setErr('Name required.'); return; }
    setSaving(true);
    try { await addLocation(campaignName, { name: f.name, description: f.description, mapUrl: f.mapUrl || undefined }); onCreated(); }
    catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>🗺 Add Location</h2>
        <div className="form-row"><label>Name *</label><input value={f.name} onChange={e => s('name', e.target.value)} /></div>
        <div className="form-row"><label>Description</label><textarea rows={3} value={f.description} onChange={e => s('description', e.target.value)} /></div>
        <div className="form-row"><label>Map URL</label><input value={f.mapUrl} onChange={e => s('mapUrl', e.target.value)} placeholder="https://…" /></div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add'}</button>
        </div>
      </div>
    </div>
  );
}
