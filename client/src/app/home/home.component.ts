import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registeredMode = false;
  users: any;
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }

  registeredToggle() {
    this.registeredMode = !this.registeredMode;
  }

  getUsers() {
    this.http.get(`${environment.apiUrl}users`).subscribe(response => {
      this.users = response;
    }, error => {
      console.error(error);
    });
  }

  cancelRegisterModel(event: boolean) {
    this.registeredMode = event;
  }
}
