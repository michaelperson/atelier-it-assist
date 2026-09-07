import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

/* Composant repris du prototype de 2023. Non retouché depuis. */
@Component({
  selector: 'ia-tableau-bord',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="data">
      <h2>Tableau de bord</h2>
      <div *ngFor="let k of keys()">
        {{ k }} : {{ data[k] }}
        <span *ngIf="data[k] > total() / keys().length">(au-dessus de la moyenne)</span>
      </div>
      <p>Total : {{ total() }}</p>
      <p *ngIf="critiques.length > 0" style="color:red">
        {{ critiques.length }} ticket(s) critique(s) ouverts depuis plus de
        {{ seuil }} jours
      </p>
    </div>
  `,
})
export class TableauBordComponent implements OnInit {
  data: any = null;
  critiques: any[] = [];
  seuil = 3;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get('/api/rapports/par-categorie').subscribe((res: any) => {
      this.data = res;
    });

    this.http.get('/api/tickets').subscribe((res: any) => {
      const maintenant = new Date();
      for (let i = 0; i < res.length; i++) {
        const t = res[i];
        if (t.priorite == 'Critique' && t.statut != 'Resolu' && t.statut != 'Ferme') {
          const ouvert = new Date(t.ouvertLe);
          const jours = (maintenant.getTime() - ouvert.getTime()) / 86400000;
          if (jours > this.seuil) {
            this.critiques.push(t);
          }
        }
      }
    });
  }

  keys() {
    if (this.data == null) return [];
    return Object.keys(this.data);
  }

  total() {
    let t = 0;
    for (const k of this.keys()) {
      t = t + this.data[k];
    }
    return t;
  }
}
