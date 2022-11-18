import { LocalInputManager, Message } from "./inputremoting.js";

export class Receiver extends LocalInputManager {
    /**
     * @param {RTCDataChannel} channel 
     */
    constructor(channel) {
        if(!channel){
            throw new Error('channel is null');
        }
        super();
        this._channel = channel
        this._channel.onmessage = this.OnMessage.bind(this)
    }
    
    /**
     * @returns {InputDevice[]}
     */
    get devices() {
        return [];
    }

    /**
     * @param {RTCDataChannel} channel 
     * @param {MessageEvent} event 
     */
    OnMessage(event){
        const e = new CustomEvent(
            'gamemessage', {detail: { message: Message.create(event.data) }});
        super.onEvent.dispatchEvent(e);
    }
}