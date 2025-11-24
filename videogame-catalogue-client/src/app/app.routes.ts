import { Routes } from '@angular/router';
import { GameListComponent } from './components/game-list/game-list.component';
import { GameEditComponent } from './components/game-edit/game-edit.component';

export const routes: Routes = [
  { path: '', component: GameListComponent },
  { path: 'edit/:id', component: GameEditComponent },
  { path: '**', redirectTo: '' }
];
