// ─── Enums ────────────────────────────────────────────────────────────────────

export type HeroClass =
  | 'Barbarian' | 'Bard' | 'Cleric' | 'Druid' | 'Fighter' | 'Monk'
  | 'Paladin' | 'Ranger' | 'Rogue' | 'Sorcerer' | 'Warlock' | 'Wizard';

export type Race =
  | 'Human' | 'Orc' | 'HalfOrc' | 'HalfElf' | 'Elf' | 'Thiefling' | 'Aasimar';

export type Alignment = number; // Flags enum

export type DiceType = 'D4' | 'D6' | 'D8' | 'D10' | 'D12' | 'D20' | 'D100';

export type QuestStatus = 'Active' | 'Completed' | 'Failed';

export type CampaignMemberRole = 'DungeonMaster' | 'Player';

export type CombatParticipantType = 'Character' | 'Monster';

// ─── Value Objects ────────────────────────────────────────────────────────────

export interface AbilityScores {
  strength: number;
  dexterity: number;
  constitution: number;
  intelligence: number;
  wisdom: number;
  charisma: number;
}

export interface CharacterCondition {
  name: string;
  remainingRounds?: number;
}

export interface InventoryItem {
  id: string;
  name: string;
  quantity: number;
  notes?: string;
}

export interface CharacterSpellEntry {
  spellId: number;
  spellName: string;
  isPrepared: boolean;
}

export interface SpellSlotUsage {
  slotLevel: number;
  slotsTotal: number;
  slotsSpent: number;
}

export interface CombatantState {
  id: string;
  name: string;
  type: CombatParticipantType;
  initiative: number;
  currentHitPoints: number;
  maxHitPoints: number;
  temporaryHitPoints: number;
  hideHitPointsFromPlayers: boolean;
  conditions: CharacterCondition[];
}

export interface CombatState {
  round: number;
  turnIndex: number;
  initiativeOrder: CombatantState[];
}

export interface MonsterStatBlock {
  id: string;
  name: string;
  creatureType: string;
  armorClass: number;
  hitPoints: number;
  challengeRating: number;
  experiencePoints: number;
  initiativeModifier: number;
  speed: number;
  notes?: string;
}

export interface SessionLogEntry {
  id: string;
  date: string;
  durationMinutes: number;
  summary: string;
  dungeonMasterNotes: string;
}

export interface CampaignTimelineEntry {
  id: string;
  occurredAt: string;
  description: string;
}

export interface NpcResource {
  id: string;
  name: string;
  role: string;
  notes: string;
}

export interface LocationResource {
  id: string;
  name: string;
  description: string;
  mapUrl?: string;
}

export interface QuestResource {
  id: string;
  title: string;
  status: QuestStatus;
  description: string;
}

export interface LootResource {
  id: string;
  name: string;
  isMagicItem: boolean;
  notes: string;
}

export interface CampaignMember {
  userId: string;
  displayName: string;
  role: CampaignMemberRole;
}

// ─── DTOs ─────────────────────────────────────────────────────────────────────

export interface CampaignDto {
  campaignName: string;
  campaignDescription: string;
}

export interface CharacterSheetDto {
  id: string;
  name: string;
  class: HeroClass;
  race: Race;
  alignment: number;
  level: number;
  experience: number;
  isNonPlayerCharacter: boolean;
  abilityScores: AbilityScores;
  currentHitPoints: number;
  maxHitPoints: number;
  temporaryHitPoints: number;
  armorClass: number;
  initiative: number;
  speed: number;
  hitDice: DiceType;
  inventory: InventoryItem[];
  equipment: InventoryItem[];
  spellbook: CharacterSpellEntry[];
  spellSlots: SpellSlotUsage[];
  conditions: CharacterCondition[];
  notes: string;
  background: string;
}

export interface CampaignTrackerDto {
  campaignName: string;
  campaignDescription: string;
  characters: CharacterSheetDto[];
  monsterLibrary: MonsterStatBlock[];
  activeCombat?: CombatState;
  sessionLogs: SessionLogEntry[];
  timelineEntries: CampaignTimelineEntry[];
  npcs: NpcResource[];
  locations: LocationResource[];
  quests: QuestResource[];
  loot: LootResource[];
  members: CampaignMember[];
}

export interface DiceRollResult {
  expression: string;
  total: number;
  rolls: number[];
  modifier: number;
  context?: string;
}
