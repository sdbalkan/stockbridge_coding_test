import { Injectable } from "@angular/core";
import { User } from "../_models/user";
import { SocketMessage } from "../_models/socket-message";
import { Subject } from "rxjs";
import { WebsocketService } from "./websocket.service";
import { map } from "rxjs/operators";
import { SocketMessageType } from "../_models/enums";
import { AuthService } from "./auth.service";

let serviceUrlFormat = "ws://66.70.229.82:8181/?{0}";
let serviceUrl = "";

@Injectable()
export class LogoutService {
  public messages: Subject<SocketMessage> | undefined;

  constructor(private websocketService: WebsocketService, private authService: AuthService) {
  }

  public connect() {
    let user: any;
    this.authService.user.subscribe(x => user = x);

    if (!user) {
      return;
    }

    serviceUrl = serviceUrlFormat.replace("{0}", user.token || "");
    this.messages = <Subject<SocketMessage>>this.websocketService
      .connect(serviceUrl)
      .pipe(
        map(
          (response: MessageEvent): SocketMessage => {
            let msg = JSON.parse(response.data);
            this.sendPing();

            return msg;
          }
        )
      );
  }

  public sendPing() {
    let pingMsg: any = {
      messageType: SocketMessageType.ping,
    };

    this.messages?.next(pingMsg);
  }
}