import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllCampaigns, createCampaign } from '../api/api';
import type { CampaignDto } from '../api/types';
import styles from './CampaignsPage.module.css';

export default function CampaignsPage() {
  const [campaigns, setCampaigns] = useState<CampaignDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showCreate, setShowCreate] = useState(false);

  const load = () => {
    setLoading(true);
    getAllCampaigns()
      .then(setCampaigns)
      .catch(() => setError('Failed to load campaigns.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  return (
    <div>
      <div className={styles.header}>
        <h1>Campaigns</h1>
        <button className="btn btn-primary" onClick={() => setShowCreate(true)}>+ New Campaign</button>
      </div>
      <div className="ornament">✦ ─────────────────── ✦</div>

      {loading && <p className="text-dim">Loading…</p>}
      {error && <p className="error-msg">{error}</p>}

      <div className={styles.grid}>
        {campaigns.map(c => (
          <Link key={c.campaignName} to={`/campaign/${encodeURIComponent(c.campaignName)}`} className={styles.card}>
            <div className={styles.cardIcon}>🗺</div>
            <h2 className={styles.cardTitle}>{c.campaignName}</h2>
            <p className={styles.cardDesc}>{c.campaignDescription || <em>No description.</em>}</p>
            <span className="badge badge-gold">Open →</span>
          </Link>
        ))}
        {!loading && campaigns.length === 0 && (
          <p className="text-dim">No campaigns yet. Create one!</p>
        )}
      </div>

      {showCreate && (
        <CreateCampaignModal
          onClose={() => setShowCreate(false)}
          onCreated={() => { setShowCreate(false); load(); }}
        />
      )}
    </div>
  );
}

function CreateCampaignModal({ onClose, onCreated }: { onClose: () => void; onCreated: () => void }) {
  const [form, setForm] = useState({ campaignName: '', campaignDescription: '', campaignImage: '', isActive: true });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);

  const set = (k: string, v: string | boolean) => setForm(f => ({ ...f, [k]: v }));

  const submit = async () => {
    if (!form.campaignName.trim()) { setErr('Campaign name is required.'); return; }
    setSaving(true);
    try {
      await createCampaign({ ...form, createdDate: new Date().toISOString() });
      onCreated();
    } catch (e: any) {
      setErr(e?.response?.data?.detail ?? 'Failed to create campaign.');
    } finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>⚔ Create Campaign</h2>
        <div className="form-row"><label>Campaign Name *</label><input value={form.campaignName} onChange={e => set('campaignName', e.target.value)} /></div>
        <div className="form-row"><label>Description</label><textarea rows={3} value={form.campaignDescription} onChange={e => set('campaignDescription', e.target.value)} /></div>
        <div className="form-row"><label>Image URL</label><input value={form.campaignImage} onChange={e => set('campaignImage', e.target.value)} placeholder="https://…" /></div>
        <div className="form-row flex items-center gap-sm">
          <input type="checkbox" id="active" checked={form.isActive} onChange={e => set('isActive', e.target.checked)} style={{ width: 'auto' }} />
          <label htmlFor="active" style={{ marginBottom: 0 }}>Active</label>
        </div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Creating…' : 'Create'}</button>
        </div>
      </div>
    </div>
  );
}
