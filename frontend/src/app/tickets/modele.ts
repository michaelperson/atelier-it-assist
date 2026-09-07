export type Priorite = 'Basse' | 'Normale' | 'Haute' | 'Critique';

export type StatutTicket =
  | 'Nouveau'
  | 'EnCours'
  | 'EnAttenteDemandeur'
  | 'Resolu'
  | 'Ferme';

export interface Ticket {
  readonly id: string;
  readonly titre: string;
  readonly categorie: string;
  readonly priorite: Priorite;
  readonly statut: StatutTicket;
  readonly demandeur: string;
  readonly service: string;
  readonly site: string;
  readonly ouvertLe: string;
  readonly description: string;
  readonly resolution: string | null;
}

export interface Escalade {
  readonly ticket: string;
  readonly equipe: string | null;
  readonly escaladeRequise: boolean;
  readonly prioriteCible: Priorite;
}
