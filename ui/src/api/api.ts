import client from './client';
import type {
  CampaignDto,
  CampaignTrackerDto,
  DiceRollResult,
  CharacterSheetDto,
} from './types';

// ─── Campaigns ────────────────────────────────────────────────────────────────

export const getAllCampaigns = (): Promise<CampaignDto[]> =>
  client.get('/api/campaign').then(r => r.data);

export const getCampaign = (name: string): Promise<CampaignDto> =>
  client.get(`/api/campaign/${encodeURIComponent(name)}`).then(r => r.data);

export const getCampaignTracker = (name: string, viewerUserId?: string): Promise<CampaignTrackerDto> => {
  const params = viewerUserId ? { viewerUserId } : {};
  return client.get(`/api/campaign/${encodeURIComponent(name)}/tracker`, { params }).then(r => r.data);
};

export const createCampaign = (data: {
  campaignName: string;
  campaignDescription: string;
  campaignImage: string;
  createdDate: string;
  isActive: boolean;
}) => client.post('/api/campaign', data);

// ─── Heroes ───────────────────────────────────────────────────────────────────

export const addHero = (campaignName: string, hero: Omit<CharacterSheetDto, 'id'> & { hitPoints: number; hitDice: string; isNonPlayerCharacter?: boolean }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/heroes`, { hero });

// ─── Characters ───────────────────────────────────────────────────────────────

export const updateCharacterHp = (campaignName: string, characterId: string, data: { damage: number; healing: number; temporaryHitPointsDelta: number }) =>
  client.patch(`/api/campaign/${encodeURIComponent(campaignName)}/characters/${characterId}/hp`, data);

export const addCharacterCondition = (campaignName: string, characterId: string, condition: string, remainingRounds?: number) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/characters/${characterId}/conditions`, { condition, remainingRounds });

// ─── Monsters ─────────────────────────────────────────────────────────────────

export const addMonster = (campaignName: string, data: {
  name: string; creatureType: string; armorClass: number; hitPoints: number;
  challengeRating: number; experiencePoints: number; initiativeModifier: number; speed: number; notes?: string;
}) => client.post(`/api/campaign/${encodeURIComponent(campaignName)}/monsters`, data);

// ─── Combat ───────────────────────────────────────────────────────────────────

export const startCombat = (campaignName: string, combatants: Array<{
  id: string; name: string; type: string; initiative: number;
  currentHitPoints: number; maxHitPoints: number; temporaryHitPoints: number;
  hideHitPointsFromPlayers: boolean; conditions?: Array<{ name: string; remainingRounds?: number }>;
}>) => client.post(`/api/campaign/${encodeURIComponent(campaignName)}/combat/start`, { combatants });

export const advanceCombatTurn = (campaignName: string) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/combat/advance`);

export const reorderCombat = (campaignName: string, combatantId: string, targetIndex: number) =>
  client.patch(`/api/campaign/${encodeURIComponent(campaignName)}/combat/reorder`, { combatantId, targetIndex });

export const updateCombatHp = (campaignName: string, data: { combatantId: string; damage: number; healing: number; temporaryHitPointsDelta: number }) =>
  client.patch(`/api/campaign/${encodeURIComponent(campaignName)}/combat/hp`, data);

export const addCombatCondition = (campaignName: string, combatantId: string, condition: string, remainingRounds?: number) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/combat/conditions`, { combatantId, condition, remainingRounds });

// ─── Resources ────────────────────────────────────────────────────────────────

export const addNpc = (campaignName: string, data: { name: string; role: string; notes: string }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/resources/npcs`, data);

export const addLocation = (campaignName: string, data: { name: string; description: string; mapUrl?: string }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/resources/locations`, data);

export const addQuest = (campaignName: string, data: { id?: string; title: string; status: string; description: string }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/resources/quests`, data);

export const addLoot = (campaignName: string, data: { name: string; isMagicItem: boolean; notes: string }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/resources/loot`, data);

// ─── Sessions ─────────────────────────────────────────────────────────────────

export const addSessionLog = (campaignName: string, data: { date: string; durationMinutes: number; summary: string; dungeonMasterNotes: string }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/sessions`, data);

// ─── Members ──────────────────────────────────────────────────────────────────

export const addMember = (campaignName: string, data: { userId: string; displayName: string; role: string }) =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/members`, data);

// ─── Dice ─────────────────────────────────────────────────────────────────────

export const rollDice = (campaignName: string, data: { expression: string; modifier: number; context?: string }): Promise<DiceRollResult> =>
  client.post(`/api/campaign/${encodeURIComponent(campaignName)}/dice/roll`, data).then(r => r.data);
