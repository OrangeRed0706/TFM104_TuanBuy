Vue.component('test', {
    // options
    //< img v - bind: src = "user.picPath" alt = "頭像" id = "testUserImg" width = "100" height = "100" >
    template: `
               <div class="col-lg-2">
                <div>
                    <img v-bind:src="user.picPath" alt="頭像" id="testUserImg" width="100" height="100"  class="Largeheadsticker" >
                </div>
                <div class="accordion" id="accordionExample">
                    <div class="collapsecard">
                        <div class="collapsecardhead" id="headingOne">
                            <h2 class="mb-0 bgcf5">
                                <button class="btn btn-link" type="button" data-toggle="collapse"
                                    data-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">
                                    我的帳戶
                                </button>
                            </h2>
                        </div>
                        <div id="collapseOne" class="collapse" aria-labelledby="headingOne"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/Index">個人檔案</a>
                            </div>
                        </div>
                        <div id="collapseOne" class="collapse" aria-labelledby="headingOne"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/ResetPassword">更改密碼</a>
                            </div>
                        </div>
                    </div>
                    <div class="collapsecard">
                        <div class="collapsecardhead" id="headingTwo">
                            <h2 class="mb-0 bgcf5">
                                <button class="btn btn-link collapsed" type="button" data-toggle="collapse"
                                    data-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                                    購買清單
                                </button>
                            </h2>
                        </div>
                        <div id="collapseTwo" class="collapse" aria-labelledby="headingTwo"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/MyBuyProduct">我的商品</a>
                            </div>
                        </div>
                    </div>
                    <div class="collapsecard">
                        <div class="collapsecardhead" id="headingThree">
                            <h2 class="mb-0 bgcf5">
                                <button class="btn btn-link collapsed" type="button" data-toggle="collapse"
                                    data-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                                    通知總攬
                                </button>
                            </h2>
                        </div>
                        <div id="collapseThree" class="collapse" aria-labelledby="headingThree"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/MyNotify">通知</a>
                            </div>
                        </div>
                    </div>
                    <div class="collapsecard">
                        <div class="collapsecardhead" id="headingFour">
                            <h2 class="mb-0 bgcf5">
                                <button class="btn btn-link collapsed" type="button" data-toggle="collapse"
                                    data-target="#collapseFour" aria-expanded="false" aria-controls="collapseFour">
                                    團購優惠券
                                </button>
                            </h2>
                        </div>
                        <div id="collapseFour" class="collapse" aria-labelledby="headingFour"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/Coupon">我的優惠券</a>
                            </div>
                        </div>
                    </div>

                    <div class="collapsecard">
                        <div class="collapsecardhead" id="headingFiVE">
                            <h2 class="mb-0 bgcf5">
                                <button class="btn btn-link collapsed" type="button" data-toggle="collapse"
                                    data-target="#collapseFive" aria-expanded="false" aria-controls="collapseFive">
                                    我是賣家
                                </button>
                            </h2>
                        </div>
                        <div id="collapseFive" class="collapse" aria-labelledby="headingFiVE"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/MyProduct">我的商品</a>
                            </div>
                        </div>

                        <div id="collapseFive" class="collapse" aria-labelledby="headingFiVE"
                            data-parent="#accordionExample">
                            <div class="collapsecard-body bgcf5">
                                <a href="/MemberCenter/OrderManger">訂單管理</a>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
  `,
    props: {
        user: {
            name: "",
            phone: "",
            birth: "",
            address: "",
            sex: "",
            picPath: ""
        }
    },
    methods: {
        getUserDate: function () {
            let self = this;
            axios.get("api/MemberCenter/").then((resp) => { this.user = resp.data });
        }
    }
});