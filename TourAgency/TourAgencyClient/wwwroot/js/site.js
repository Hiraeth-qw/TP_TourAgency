function showToast(message, isSuccess = true) {
    var toastElement = $('#liveToast');
    var toastBody = $('#toastMessage');

    toastElement.removeClass('bg-primary bg-danger bg-warning');

    if (isSuccess) {
        toastElement.addClass('bg-primary');
    } else {
        toastElement.addClass('bg-danger');
    }

    toastBody.text(message);

    var toast = new bootstrap.Toast(toastElement);
    toast.show();
}

$(document).ready(function () {
    $('.tour-details-btn').on('click', function (e) {
        e.preventDefault();
        var tourId = $(this).data('tour-id');
        var modalContentContainer = $('#tourDetailsModal .modal-content');

        modalContentContainer.html('<div class="modal-body text-center p-5"><div class="spinner-border text-primary"></div></div>');

        $.ajax({
            url: '/Tour/Details/' + tourId,
            type: 'GET',
            success: function (data) {
                modalContentContainer.html(data);
            },
            error: function () {
                modalContentContainer.html('<div class="modal-body text-danger">Ошибка загрузки данных.</div>');
            }
        });
    });

    $(document).on('click', '.open-quantity-modal-btn', function () {
        var tourId = $(this).data('tour-id');
        var tourTitle = $(this).data('tour-title');
        var maxSeats = $(this).data('max-seats');

        $('#quantityModalTitle').text(tourTitle);
        $('#hiddenTourId').val(tourId);
        $('#maxSeatsDisplay').text(maxSeats);

        var input = $('#touristsNumber');
        input.attr('max', maxSeats);
        input.val(1);
        $('#quantityError').hide();

        $('#tourDetailsModal').modal('hide');

        setTimeout(function () {
            var quantityModal = new bootstrap.Modal(document.getElementById('quantitySelectionModal'));
            quantityModal.show();
        }, 150);
    });

    $('#addToCartForm').on('submit', function (e) {
        e.preventDefault();

        var form = $(this);
        var submitButton = form.find('#submitAddToCart');
        var errorSpan = $('#quantityError');

        submitButton.prop('disabled', true).text('Добавление...');
        errorSpan.hide();

        var formData = {
            TourId: parseInt(form.find('#hiddenTourId').val()),
            TouristsNumber: parseInt(form.find('#touristsNumber').val())
        };

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                $('#quantitySelectionModal').modal('hide');

                submitButton.prop('disabled', false).text('Подтвердить');

                showToast(response.message, true);
            },
            error: function (xhr) {
                var errorMsg = "Ошибка при добавлении.";

                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMsg = xhr.responseJSON.message;
                } else if (xhr.status === 401) {
                    errorMsg = "Необходимо войти в систему.";
                }

                errorSpan.text(errorMsg).show();

                submitButton.prop('disabled', false).text('Подтвердить');
            }
        });
    });
});