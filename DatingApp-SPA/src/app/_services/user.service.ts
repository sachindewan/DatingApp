import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PagenatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.baseUrl + 'user/';
  constructor(private http: HttpClient) {}
  getUsers(
    page?,
    itemPerPage?,
    userParams?,
    likeParams?
  ): Observable<PagenatedResult<User[]>> {
    const pagenatedResult: PagenatedResult<User[]> = new PagenatedResult<
      User[]
    >();

    let params = new HttpParams();

    if (page != null && itemPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }
    if (userParams != null && Object.keys(userParams).length > 0) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }
    if (likeParams === 'Likers') {
      params = params.append('likers', 'true');
    }
    if (likeParams === 'Likees') {
      params = params.append('likees', 'true');
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
  sendLike(id: number, reciepentId: number) {
    return this.http.post(this.baseUrl + id + '/like/' + reciepentId, {});
  }
  getMessages(id: number, page?, itemPerPage?, messageContainer?) {
    const pagenatedResult: PagenatedResult<Message[]> = new PagenatedResult<
      Message[]
    >();

    let params = new HttpParams();
    params = params.append('messageContainer', messageContainer);
    if (page != null && itemPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }
    return this.http
      .get<Message[]>(this.baseUrl + id + '/messages', {
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
  getMessageThread(userId: number, reciepentId: number) {
    return this.http.get<Message[]>(
      this.baseUrl + userId + '/messages/thread/' + reciepentId
    );
  }
  sendMessage(id: number, newMessage: any) {
    return this.http.post<Message>(this.baseUrl + id + '/messages', newMessage);
  }
  deleteMessage(messageId: number, userId: number) {
    return this.http.post(this.baseUrl + userId + '/messages/' + messageId, {});
  }
  markAsRead(messageId: number, userId: number) {
    return this.http
      .post(this.baseUrl + userId + '/messages/' + messageId + '/read', {})
      .subscribe();
  }
}
