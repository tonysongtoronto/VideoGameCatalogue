import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { VideoGame } from '../../models/video-game.model';
import { VideoGameService } from '../../services/video-game.service';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './game-list.component.html',
  styleUrls: ['./game-list.component.css']
})
export class GameListComponent implements OnInit {
  games: VideoGame[] = [];
  filteredGames: VideoGame[] = [];
  loading = true;
  error: string | null = null;

  // 查询相关属性
  searchTerm: string = '';
  filterGenre: string = '';
  filterPlatform: string = '';
  sortBy: string = 'title';

  // 用于下拉选项
  allGenres: string[] = [];
  allPlatforms: string[] = [];

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
        this.extractFilters();
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load games';
        this.loading = false;
        console.error(err);
      }
    });
  }

  extractFilters(): void {
    // 提取所有唯一的类型
    this.allGenres = [...new Set(this.games.map(g => g.genre))].sort();
    // 提取所有唯一的平台
    this.allPlatforms = [...new Set(this.games.map(g => g.platform))].sort();
  }

  applyFilters(): void {
    this.filteredGames = this.games
      .filter(game => {
        const matchesSearch =
          game.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
          game.publisher.toLowerCase().includes(this.searchTerm.toLowerCase());
        const matchesGenre = !this.filterGenre || game.genre === this.filterGenre;
        const matchesPlatform = !this.filterPlatform || game.platform === this.filterPlatform;
        return matchesSearch && matchesGenre && matchesPlatform;
      })
      .sort((a, b) => {
        switch (this.sortBy) {
          case 'title':
            return a.title.localeCompare(b.title);
          case 'price-low':
            return a.price - b.price;
          case 'price-high':
            return b.price - a.price;
          case 'date-new':
            return new Date(b.releaseDate).getTime() - new Date(a.releaseDate).getTime();
          case 'date-old':
            return new Date(a.releaseDate).getTime() - new Date(b.releaseDate).getTime();
          default:
            return 0;
        }
      });
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.filterGenre = '';
    this.filterPlatform = '';
    this.sortBy = 'title';
    this.applyFilters();
  }

  // 辅助方法：将平台名称转换为 CSS 类名
  getPlatformClass(platform: string): string {
    return 'platform-' + platform.toLowerCase().replace(/\s+/g, '-');
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
