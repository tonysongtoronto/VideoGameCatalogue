import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { VideoGame } from '../../models/video-game.model';
import { VideoGameService } from '../../services/video-game.service';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-list.component.html',
  styleUrls: ['./game-list.component.css']
})
export class GameListComponent implements OnInit {
  games: VideoGame[] = [];
  loading = true;
  error: string | null = null;

  constructor(
    private gameService: VideoGameService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadGames();
  }

  loadGames(): void {
    this.loading = true;
    this.error = null;
    this.gameService.getAll().subscribe({
      next: (data) => {
        this.games = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load games';
        this.loading = false;
        console.error(err);
      }
    });
  }

  editGame(id: number): void {
    this.router.navigate(['/edit', id]);
  }

  createGame(): void {
    this.router.navigate(['/edit', 'new']);
  }

  deleteGame(id: number): void {
    if (confirm('Are you sure you want to delete this game?')) {
      this.gameService.delete(id).subscribe({
        next: () => this.loadGames(),
        error: (err) => {
          this.error = 'Failed to delete game';
          console.error(err);
        }
      });
    }
  }
}
