export interface Message {
  id: number;
  senderId: number;
  senderKnownAs: string;
  senderPhotoUrl: string;
  recieverId: number;
  recieverKnownAs: string;
  recieverPhotoUrl: string;
  content: string;
  isRead: boolean;
  dateRead: Date;
  messageSent: Date;
}
