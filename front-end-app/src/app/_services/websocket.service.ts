import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { AnonymousSubject } from "rxjs/internal/Subject";
import { Observer } from "rxjs/internal/types";


@Injectable()
export class WebsocketService {
  constructor() {

  }

  private subject: AnonymousSubject<MessageEvent> | undefined;

  public connect(url: string): AnonymousSubject<MessageEvent> {
    if (!this.subject) {
      this.subject = this.create(url);
      console.log("Successfully connected: " + url);
    }

    return this.subject;
  }

  private create(url: string): AnonymousSubject<MessageEvent> {
    let ws = new WebSocket(url);

    let observable = new Observable((obs: Observer<MessageEvent>) => {
      ws.onmessage = obs.next.bind(obs);
      ws.onerror = obs.error.bind(obs);
      ws.onclose = obs.complete.bind(obs);

      return ws.close.bind(ws);
    });

    let observer = {
      error: () => null,
      complete: () => null,
      next: (data: Object) => {
        console.log("Sending: " + JSON.stringify(data));
        console.log("ReadyState: " + ws.readyState);
        if (ws.readyState === WebSocket.OPEN) {
          ws.send(JSON.stringify(data));
        }
      }
    };

    return new AnonymousSubject<MessageEvent>(observer, observable);
  }
}