import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LibellePrioritePipe } from '../partage/libelle-priorite.pipe';
import { Ticket } from './modele';
import { TicketService } from './ticket.service';

/**
 * Composant de référence du projet : autonome, état en signaux, souscription
 * détachée avec le composant, aucun appel HTTP direct, aucun `any`.
 */
@Component({
  selector: 'ia-liste-tickets',
  standalone: true,
  imports: [LibellePrioritePipe],
  template: `
    <label>
      Filtrer par service
      <select [value]="filtre()" (change)="majFiltre($event)">
        <option value="">Tous les services</option>
        @for (service of services(); track service) {
          <option [value]="service">{{ service }}</option>
        }
      </select>
    </label>

    @if (chargement()) {
      <p>Chargement…</p>
    } @else if (erreur()) {
      <p role="alert">{{ erreur() }}</p>
    } @else {
      <p>{{ visibles().length }} ticket(s)</p>
      <ul>
        @for (ticket of visibles(); track ticket.id) {
          <li>
            <strong>{{ ticket.id }}</strong>
            {{ ticket.titre }}
            <em>{{ ticket.priorite | libellePriorite }}</em>
          </li>
        }
      </ul>
    }
  `,
})
export class ListeTicketsComponent {
  private readonly service = inject(TicketService);

  protected readonly tickets = signal<readonly Ticket[]>([]);
  protected readonly filtre = signal('');
  protected readonly chargement = signal(true);
  protected readonly erreur = signal<string | null>(null);

  protected readonly services = computed(() =>
    [...new Set(this.tickets().map((ticket) => ticket.service))].sort((serviceA, serviceB) =>
      serviceA.localeCompare(serviceB, 'fr'),
    ),
  );

  protected readonly visibles = computed(() => {
    const serviceSelectionne = this.filtre();
    if (serviceSelectionne === '') {
      return this.tickets();
    }
    return this.tickets().filter((ticket) => ticket.service === serviceSelectionne);
  });

  constructor() {
    this.service
      .lister()
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: (tickets) => {
          this.tickets.set(tickets);
          this.chargement.set(false);
        },
        error: () => {
          this.erreur.set('Impossible de charger les tickets.');
          this.chargement.set(false);
        },
      });
  }

  protected majFiltre(evenement: Event): void {
    this.filtre.set((evenement.target as HTMLSelectElement).value);
  }
}
