import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getCampaignTracker } from '../api/api';
import type { CampaignTrackerDto } from '../api/types';
import CharactersPanel from '../components/CharactersPanel';
import CombatPanel from '../components/CombatPanel';
import QuestsPanel from '../components/QuestsPanel';
import NpcsPanel from '../components/NpcsPanel';
import LocationsPanel from '../components/LocationsPanel';
import LootPanel from '../components/LootPanel';
import SessionsPanel from '../components/SessionsPanel';
import MonstersPanel from '../components/MonstersPanel';
import DicePanel from '../components/DicePanel';
import styles from './CampaignPage.module.css';

type Tab = 'characters' | 'combat' | 'quests' | 'npcs' | 'locations' | 'loot' | 'sessions' | 'monsters' | 'dice';

const TABS: { id: Tab; label: string; icon: string }[] = [
  { id: 'characters', label: 'Characters', icon: '🧝' },
  { id: 'combat', label: 'Combat', icon: '⚔' },
  { id: 'quests', label: 'Quests', icon: '📜' },
  { id: 'npcs', label: 'NPCs', icon: '🧙' },
  { id: 'locations', label: 'Locations', icon: '🗺' },
  { id: 'loot', label: 'Loot', icon: '💎' },
  { id: 'sessions', label: 'Sessions', icon: '📖' },
  { id: 'monsters', label: 'Monsters', icon: '👾' },
  { id: 'dice', label: 'Dice', icon: '🎲' },
];

export default function CampaignPage() {
  const { name } = useParams<{ name: string }>();
  const [tracker, setTracker] = useState<CampaignTrackerDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [tab, setTab] = useState<Tab>('characters');

  const load = () => {
    if (!name) return;
    setLoading(true);
    getCampaignTracker(decodeURIComponent(name))
      .then(setTracker)
      .catch(() => setError('Campaign not found.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [name]);

  if (loading) return <p className="text-dim">Loading campaign…</p>;
  if (error || !tracker) return (
    <div>
      <p className="error-msg">{error || 'Not found.'}</p>
      <Link to="/" className="btn" style={{ marginTop: '1rem', display: 'inline-block' }}>← Back</Link>
    </div>
  );

  return (
    <div>
      <div className={styles.pageHeader}>
        <div>
          <Link to="/" className="text-dim" style={{ fontSize: '.8rem' }}>← Campaigns</Link>
          <h1 style={{ marginTop: '.2rem' }}>{tracker.campaignName}</h1>
          {tracker.campaignDescription && <p className="text-dim" style={{ marginTop: '.2rem' }}>{tracker.campaignDescription}</p>}
        </div>
        <button className="btn btn-sm" onClick={load}>↻ Refresh</button>
      </div>

      <div className="ornament">✦ ─────────────────────────────────── ✦</div>

      <nav className={styles.tabs}>
        {TABS.map(t => (
          <button
            key={t.id}
            className={`${styles.tab} ${tab === t.id ? styles.tabActive : ''}`}
            onClick={() => setTab(t.id)}
          >
            <span>{t.icon}</span>
            <span>{t.label}</span>
          </button>
        ))}
      </nav>

      <div className={styles.content}>
        {tab === 'characters' && <CharactersPanel campaignName={tracker.campaignName} characters={tracker.characters} onRefresh={load} />}
        {tab === 'combat' && <CombatPanel campaignName={tracker.campaignName} tracker={tracker} onRefresh={load} />}
        {tab === 'quests' && <QuestsPanel campaignName={tracker.campaignName} quests={tracker.quests} onRefresh={load} />}
        {tab === 'npcs' && <NpcsPanel campaignName={tracker.campaignName} npcs={tracker.npcs} onRefresh={load} />}
        {tab === 'locations' && <LocationsPanel campaignName={tracker.campaignName} locations={tracker.locations} onRefresh={load} />}
        {tab === 'loot' && <LootPanel campaignName={tracker.campaignName} loot={tracker.loot} onRefresh={load} />}
        {tab === 'sessions' && <SessionsPanel campaignName={tracker.campaignName} sessions={tracker.sessionLogs} onRefresh={load} />}
        {tab === 'monsters' && <MonstersPanel campaignName={tracker.campaignName} monsters={tracker.monsterLibrary} onRefresh={load} />}
        {tab === 'dice' && <DicePanel campaignName={tracker.campaignName} />}
      </div>
    </div>
  );
}
