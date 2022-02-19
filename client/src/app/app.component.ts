import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { setTheme } from 'ngx-bootstrap/utils';
import { User } from './_model/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;

  constructor(private accountService: AccountService) {
    setTheme('bs4');
  }

  ngOnInit() {
      this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.accountService.setCurrentUser(user);
  }


}
