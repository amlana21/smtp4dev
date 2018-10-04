import 'es6-collections';
import Vue from "vue";
import MessageList from './components/messagelist';
import SessionList from './components/sessionlist';
import MessageView from './components/messageview';
import SessionView from './components/sessionview';
import Component from "vue-class-component";
import MessageSummary from "ApiClient/MessageSummary";
import SessionSummary from "ApiClient/SessionSummary";
import Quasar from "quasar-framework/dist/quasar.mat.esm";
import "quasar-framework/dist/umd/quasar.mat.css";
import "quasar-extras/material-icons/material-icons.css";

Vue.use(Quasar); 

@Component({})
export default class Main extends Vue {

    selectedMessage: MessageSummary | null = null;
    selectedSession: SessionSummary | null = null;

    selectedMessageChanged(selectedMessage: MessageSummary | null) {
        this.selectedMessage = selectedMessage;
    };

    selectedSessionChanged(selectedSession: SessionSummary | null) {
        this.selectedSession = selectedSession;
    };

}


new Main({
    el: '#app',
    components: { messagelist: MessageList, messageview: MessageView, sessionlist: SessionList, sessionview: SessionView }
});
