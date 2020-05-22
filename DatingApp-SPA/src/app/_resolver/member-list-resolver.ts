import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 5;
  userParams: any = {};
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private router: Router
  ) {
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
  }
  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    return this.userService
      .getUsers(this.pageNumber, this.pageSize, this.userParams)
      .pipe(
        catchError((error) => {
          this.alertify.error(error);
          this.router.navigate(['/home']);
          return of(null);
        })
      );
  }
}
