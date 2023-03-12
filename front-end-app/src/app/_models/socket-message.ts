import { SocketMessageType } from "./enums";

export interface SocketMessage {
    MessageType: SocketMessageType | undefined;
    TimeStamp: Date | undefined;
}