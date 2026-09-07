import { Component } from '@angular/core';
import { ListeTicketsComponent } from './tickets/liste-tickets.component';

@Component({
  selector: 'ia-racine',
  standalone: true,
  imports: [ListeTicketsComponent],
  template: `
    <h1>IT-Assist</h1>
    <ia-liste-tickets />
  `,
})
export class AppComponent {}
