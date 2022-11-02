var Modal = {
    Events: {
        CloseModalClicked: function () {
            $("#generic-modal").modal("hide");
        }
    },
    StartUp() {
        $("#generic-modal-close").click(Modal.Events.CloseModalClicked);
    },
    PartialView(html) {
        $("#generic-modal .modal-body").html(html);
        $("#generic-modal").modal("show");
    }
};