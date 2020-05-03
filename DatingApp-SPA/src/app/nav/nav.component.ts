import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  returnUrl: string;
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {}

  login() {
    this.returnUrl = this.route.snapshot.queryParams.returnUrl || '/member';
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Logged in was successfull');
      },
      // tslint:disable-next-line: no-shadowed-variable
      (error) => {
        this.alertify.error('failed to login ' + error);
      },
      () => {
        this.router.navigate([this.returnUrl]);
      }
    );
  }
  loggedIn() {
    return this.authService.loggedIn();
  }
  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/']);
  }
}
