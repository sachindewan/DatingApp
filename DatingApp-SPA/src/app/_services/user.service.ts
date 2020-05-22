import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PagenatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.baseUrl + 'user/';
  constructor(private http: HttpClient) {}
  getUsers(
    page?,
    itemPerPage?,
    userParams?
  ): Observable<PagenatedResult<User[]>> {
    const pagenatedResult: PagenatedResult<User[]> = new PagenatedResult<
      User[]
    >();

    let params = new HttpParams();

    if (page != null && itemPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }
    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }
    return this.http
      .get<User[]>(this.baseUrl + 'users', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          pagenatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            pagenatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
            return pagenatedResult;
          }
        })
      );
  }
  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }
  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }
  setMainPhoto(userId: number, id: number) {
    return this.http.post(
      environment.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain',
      {}
    );
  }
  deletePhoto(userId: number, id: number) {
    return this.http.delete(
      environment.baseUrl + 'users/' + userId + '/photos/' + id,
      {}
    );
  }
}
