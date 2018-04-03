import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { HubConnection } from '@aspnet/signalr'
import { forEach } from '@angular/router/src/utils/collection';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    private connection: HubConnection;

    public currentEquation: string | null;
    public canAnswer: boolean = true;

    public id: string | null;
    public name: string = "Randomname";
    public score: number = 0;

    public players: Player[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.connectSignalr(baseUrl);
    }

    private connectSignalr(baseUrl: string) {
        this.connection = new HubConnection(baseUrl + 'hubs/mathgame');

        this.connection
            .start()
            .then(() => {
                this.connection.on("SendMessage", msg => this.receiveMessage(msg));
                this.connection.on("SetId", id => {
                    this.id = id;

                    console.log("Got id: " + id);

                    this.updateInfo();
                });
            }, reason => console.error("SignlaR connection error: " + reason));
    }

    public yesClick() {
        this.sendAnswer(true);
    }

    public noClick() {
        this.sendAnswer(false);
    }

    public updateInfo() {
        this.connection.invoke("UpdateInfo", { id: this.id, name: this.name });
    }

    private sendAnswer(answer: boolean) {
        this.connection.invoke("Answer", { isEquationCorrect: answer });
    }

    private receiveMessage(message: MessageWrapper) {
        console.log("Got message:");
        console.log(message)

        let msg = null;

        if (message.messageType == "RoundStartMessage") {
            msg = message.message as RoundStartMessage;

            this.currentEquation = msg.equation;
            this.canAnswer = true;
        } else if (message.messageType == "RoundEndMessage") {
            msg = message.message as RoundEndMessage;

            this.currentEquation = null;
        } else if (message.messageType == "DisableRoundMessage") {
            msg = message.message as DisableRoundMessage;

            if (this.id == msg.disableForPlayerId) {
                this.canAnswer = false;
            }
        } else if (message.messageType == "PlayerInfoUpdateMessage") {
            msg = message.message as PlayerInfoUpdateMessage;
            let updateMessage = msg as PlayerInfoUpdateMessage;

            let player = this.players.find(x => x.id == updateMessage.id);

            if (player) {
                player.id = msg.id;
                player.name = msg.name;
                player.score = msg.score;
            } else {
                let player = new Player();

                player.id = msg.id;
                player.name = msg.name;
                player.score = msg.score;

                this.players.push(player);
            }

            if (msg.id == this.id) {
                this.name = msg.name;
                this.score = msg.score;
            }

            this.sortPlayers();
        } else if (message.messageType == "PlayerRemoveMessage") {
            msg = message.message as PlayerRemoveMessage;
            let updateMessage = msg as PlayerInfoUpdateMessage;

            this.players = this.players.filter(x => x.id != updateMessage.id);

            this.sortPlayers();
        } else if (message.messageType == "FullUpdateMessage") {
            msg = message.message as FullUpdateMessage;

            this.players = [];

            for (let p of msg.playerInfos) {
                let player = new Player();

                player.id = p.id;
                player.name = p.name;
                player.score = p.score;

                this.players.push(player);
            }

            this.canAnswer = false;
            this.sortPlayers();
        }
    }

    private sortPlayers() {
        this.players = this.players.sort((a, b) => a.score == b.score ? 0 : a.score > b.score ? -1 : 1);
    }
}

class Player {
    id: string;
    name: string;
    score: number;
}

interface MessageWrapper {
    messageType: string;
    message: MathGameMessage;
}

interface MathGameMessage { }

interface RoundEndMessage extends MathGameMessage {
    winningPlayerId: string;
}

interface DisableRoundMessage extends MathGameMessage {
    disableForPlayerId: string;
}

interface RoundStartMessage extends MathGameMessage {
    equation: string;
}

interface PlayerInfoUpdateMessage extends MathGameMessage {
    id: string;
    name: string;
    score: number;
}

interface PlayerRemoveMessage extends MathGameMessage {
    id: string;
}

interface FullUpdateMessage extends MathGameMessage {
    playerInfos: PlayerInfoUpdateMessage[];
}