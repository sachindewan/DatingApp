import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MessageResolver implements Resolve<Message[]> {
  pageNumber = 1;
  pageSize = 5;
  messageContainer = 'Unread';
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private router: Router,
    private authService: AuthService
  ) {
    // this.userParams.minAge = 18;
    // this.userParams.maxAge = 99;
  }
  resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pageNumber,
        this.pageSize,
        this.messageContainer
      )
      .pipe(
        catchError((error) => {
          this.alertify.error('problem in retrieving messages');
          this.router.navigate(['/home']);
          return of(null);
        })
      );
  }
}
