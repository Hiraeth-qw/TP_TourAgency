$(document).ready(function () {
    $('.tour-details-btn').on('click', function (e) {
        e.preventDefault();

        var tourId = $(this).data('tour-id');
        var modalContentContainer = $('#tourDetailsModal .modal-content');

        modalContentContainer.html(`
            <div class="modal-body text-center p-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Загрузка...</span>
                </div>
            </div>
        `);

        $.ajax({
            url: '/Tour/Details/' + tourId,
            type: 'GET',
            success: function (data) {
                modalContentContainer.html(data);
            },
            error: function (xhr, status, error) {
                var errorMessage = 'Ошибка при загрузке деталей тура.';
                if (xhr.status === 404) {
                    errorMessage = 'Тур не найден (404).';
                }
                modalContentContainer.html(`
                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">Ошибка!</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body text-danger">
                        <p>${errorMessage}</p>
                        <p>Попробуйте обновить страницу.</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Закрыть</button>
                    </div>
                `);
            }
        });
    });
});