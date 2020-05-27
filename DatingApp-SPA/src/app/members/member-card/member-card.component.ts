import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(
    private userServce: UserService,
    private authService: AuthService,
    private alertifyService: AlertifyService
  ) {}

  ngOnInit() {}
  likeUser(id: number) {
    this.userServce
      .sendLike(this.authService.decodedToken.nameid, id)
      .subscribe(
        (data) => {
          this.alertifyService.success('you have liked' + this.user.knownAs);
        },
        (error) => this.alertifyService.error(error)
      );
  }
}
