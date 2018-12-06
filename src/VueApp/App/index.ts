import Vue from "vue";
import VueRouter from "vue-router";
import "./third-party";
import moment from "moment";
import Main from "./Main.vue";

//import './styles.scss';
import '../node_modules/bootstrap-material-design/dist/css/bootstrap-material-design.min.css';

Vue.config.productionTip = false;
Vue.use(VueRouter);

Vue.filter('formatDate', function (value: any) {
  if (value) {
      return moment(value).format('MM/DD/YYYY hh:mm:ss A')
  }
  return value;
});

let v = new Vue({
    el: '#app',
    render: h => h(Main)
});