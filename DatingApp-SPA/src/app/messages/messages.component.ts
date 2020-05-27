import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination, PagenatedResult } from '../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer: any = {};
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private router: ActivatedRoute,
    private authService: AuthService
  ) {
    this.messageContainer = 'Unread';
  }

  ngOnInit(): void {
    this.router.data.subscribe((data) => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }
  loadMessages() {
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PagenatedResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        (error) => this.alertify.error(error)
      );
  }
  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }
  deleteMessage(messageId: number, event: any) {
    event.stopPropagation();
    this.alertify.confirm(
      'are you sure? do you want to delete this message',
      () => {
        this.userService
          .deleteMessage(messageId, this.authService.decodedToken.nameid)
          .subscribe(
            (res) => {
              this.messages.splice(
                this.messages.findIndex((x) => x.id === messageId),
                1
              );
              this.alertify.success('message has been deleted');
            },
            (error) => this.alertify.error(error)
          );
      }
    );
  }
}
