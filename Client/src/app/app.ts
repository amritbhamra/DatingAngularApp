import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HttpClientModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App  implements OnInit {
  private http = inject(HttpClient);
  protected title = signal('Hello World22')  ;
  protected members = signal<any[]>([]);

   async ngOnInit() {
    console.log('App component initialized');
    // this.http.get('https://localhost:5001/api/members').subscribe({
    //   next: (response) => this.members.set(response as any[]),
    //   error: (err) => console.log(err),
    //   complete: () => console.log('Request completed')
    // });
    this.members.set(await this.loadMembers() as any[]);

  }

  async loadMembers() {
    try {
      return await lastValueFrom(this.http.get('https://localhost:5001/api/members'));
      
    } catch (err) {
      console.log(err);
      return [];
    } 
    finally 
    {      console.log('Request completed');
    }
  }
  
}
