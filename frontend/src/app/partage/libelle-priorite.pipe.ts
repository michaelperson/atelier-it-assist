import { Pipe, PipeTransform } from '@angular/core';
import { Priorite } from '../tickets/modele';

@Pipe({ name: 'libellePriorite', standalone: true })
export class LibellePrioritePipe implements PipeTransform {
  private static readonly libelles: Readonly<Record<Priorite, string>> = {
    Basse: 'Priorité basse',
    Normale: 'Priorité normale',
    Haute: 'Priorité haute',
    Critique: 'Priorité critique',
  };

  transform(valeur: Priorite): string {
    return LibellePrioritePipe.libelles[valeur];
  }
}
