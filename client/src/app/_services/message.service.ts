import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getMessages(pageNumber: number, pageSize: number, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params.append('container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessagesThread(username: string) {
    return this.http.get<any>(this.baseUrl + "messages/thread/" + username);
  }

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername: username, content: content });
  }

  deleteMessage(messageId: number) {
    return this.http.delete(this.baseUrl + 'messages/' + messageId);
  }
}
