import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VideoGame } from '../models/video-game.model';

@Injectable({
  providedIn: 'root'
})
export class VideoGameService {
  private apiUrl = 'http://localhost:5000/api/videogames';

  constructor(private http: HttpClient) {}

  getAll(): Observable<VideoGame[]> {
    return this.http.get<VideoGame[]>(this.apiUrl);
  }

  getById(id: number): Observable<VideoGame> {
    return this.http.get<VideoGame>(`${this.apiUrl}/${id}`);
  }

  create(game: VideoGame): Observable<VideoGame> {
    return this.http.post<VideoGame>(this.apiUrl, game);
  }

  update(id: number, game: VideoGame): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, game);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
