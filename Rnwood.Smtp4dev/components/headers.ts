import { Component, Prop,Watch } from 'vue-property-decorator'
import Vue from 'vue'
import Header from "../ApiClient/Header";

@Component({
    template: require('./headers.html')
})
export default class Headers extends Vue {
    constructor() {
        super(); 
    }

    @Prop({ default: null })
    headers: Header[] | null = null;

    columns = [
        {
            name: 'name',
            label: 'Name',
            align: 'left',
            field: 'name',
            sortable: true,
        },
        {
            name: 'value',
            label: 'Value',
            align: 'left',
            field: 'value',
            sortable: true,
        }
    ];
    

    async created() {

     
    }

    async destroyed() {
        
    }
}