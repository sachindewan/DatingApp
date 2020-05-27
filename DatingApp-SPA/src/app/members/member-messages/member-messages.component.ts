import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { error } from 'protractor';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
  @Input() recieverId: number;
  messages: Message[] = [];
  newMessage: any = {};
  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.loadMessages();
  }
  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService
      .getMessageThread(this.authService.decodedToken.nameid, this.recieverId)
      .pipe(
        tap((messages) => {
          for (let i = 0; i < messages.length; i++) {
            if (
              messages[i].isRead === false &&
              messages[i].recieverId === currentUserId
            ) {
              this.userService.markAsRead(messages[i].id, currentUserId);
            }
          }
        })
      )
      .subscribe(
        (response) => {
          this.messages = response;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
  sendMessage() {
    this.newMessage.recieverId = this.recieverId;
    this.userService
      .sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe(
        (messages) => {
          this.messages.unshift(messages);
          this.newMessage = '';
        },
        // tslint:disable-next-line: no-shadowed-variable
        (error) => {
          this.alertify.error(error);
        }
      );
  }
}
