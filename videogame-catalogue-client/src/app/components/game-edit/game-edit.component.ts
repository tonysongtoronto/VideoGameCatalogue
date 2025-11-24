import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { VideoGame } from '../../models/video-game.model';
import { VideoGameService } from '../../services/video-game.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-game-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './game-edit.component.html',
  styleUrls: ['./game-edit.component.css']
})
export class GameEditComponent implements OnInit {
  gameForm: FormGroup;
  isNewGame = true;
  gameId: number | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private gameService: VideoGameService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.gameForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      publisher: ['', [Validators.required, Validators.maxLength(100)]],
      genre: ['', [Validators.required, Validators.maxLength(50)]],
      platform: ['', [Validators.required, Validators.maxLength(50)]],
      releaseDate: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id && id !== 'new') {
      this.isNewGame = false;
      this.gameId = parseInt(id, 10);
      this.loadGame(this.gameId);
    }
  }

  loadGame(id: number): void {
    this.loading = true;
    this.gameService.getById(id).subscribe({
      next: (game) => {
        this.gameForm.patchValue({
          title: game.title,
          publisher: game.publisher,
          genre: game.genre,
          platform: game.platform,
          releaseDate: game.releaseDate.split('T')[0],
          price: game.price
        });
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load game';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onSubmit(): void {
    if (this.gameForm.valid) {
      this.loading = true;
      const gameData: VideoGame = {
        id: this.gameId || 0,
        ...this.gameForm.value
      };

      const operation: Observable<any> = this.isNewGame
        ? this.gameService.create(gameData)
        : this.gameService.update(this.gameId!, gameData);

      operation.subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.error = `Failed to ${this.isNewGame ? 'create' : 'update'} game`;
          this.loading = false;
          console.error(err);
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/']);
  }

  // Helper method to check if a field is invalid
  isFieldInvalid(fieldName: string): boolean {
    const field = this.gameForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}
