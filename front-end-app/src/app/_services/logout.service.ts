import { Injectable } from "@angular/core";
import { User } from "../_models/user";
import { SocketMessage } from "../_models/socket-message";
import { Subject } from "rxjs";
import { WebsocketService } from "./websocket.service";
import { map } from "rxjs/operators";
import { SocketMessageType } from "../_models/enums";

let serviceUrlFormat = "ws://66.70.229.82:8181/?{0}";
let serviceUrl = "";

@Injectable()
export class LogoutService {
  public messages: Subject<SocketMessage> | undefined;
  websocketService: WebsocketService;

  constructor(websocketService: WebsocketService) {
    this.websocketService = websocketService;
  }

  public connect() {
    let strUser = localStorage.getItem('user')
    if (!strUser) {
      return;
    }

    let user: User = JSON.parse(strUser);
    serviceUrl = serviceUrlFormat.replace("{0}", user?.token || "");
    this.messages = <Subject<SocketMessage>>this.websocketService
      .connect(serviceUrl)
      .pipe(
        map(
          (response: MessageEvent): SocketMessage => {
            console.log("Receivedddd: " + response.data);
            let msg = JSON.parse(response.data);
            this.sendPing();
            
            return msg;
          }
        )
      );
  }

  public sendPing() {
    let pingMsg: any = {
      messageType: SocketMessageType.logoff,
    };

    this.messages?.next(pingMsg);
  }
}