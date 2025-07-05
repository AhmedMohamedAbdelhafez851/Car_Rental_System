/**
 * JavaScript for CheckIn.cshtml
 */
(function () {
    let penaltyCount = 1;

    // Add a new penalty row
    function addPenalty() {
        const container = document.getElementById('penaltiesContainer');
        const newRow = `
            <div class="penalty-row mb-3">
                <div class="row g-2 align-items-center">
                    <div class="col-md-8">
                        <input type="text" name="Penalties[${penaltyCount}].Reason" class="form-control" placeholder="سبب المخالفة" />
                    </div>
                    <div class="col-md-3">
                        <input type="number" name="Penalties[${penaltyCount}].Amount" class="form-control" placeholder="المبلغ" min="0" oninput="calculateFinalAmount()" />
                    </div>
                    <div class="col-md-1">
                        <button type="button" class="btn btn-danger btn-sm" onclick="removePenalty(this)">-</button>
                    </div>
                </div>
            </div>`;
        container.insertAdjacentHTML('beforeend', newRow);
        penaltyCount++;
        calculateFinalAmount();
    }

    // Remove a penalty row
    function removePenalty(button) {
        button.closest('.penalty-row').remove();
        calculateFinalAmount();
    }

    // Calculate final amount
    function calculateFinalAmount() {
        const returnDate = new Date(document.getElementById('ReturnDate').value);
        const endDate = new Date('@Model.EndDate.ToString("yyyy-MM-ddTHH:mm")');
        const dailyRate = @Model.DailyRate;
        let delayCharge = 0;
        if (returnDate > endDate) {
            const extraDays = Math.ceil((returnDate - endDate) / (1000 * 60 * 60 * 24));
            delayCharge = extraDays * dailyRate;
        }
        document.getElementById('delayCharge').textContent = delayCharge.toFixed(0);

        const initialFuelLevel = @((int)Model.InitialFuelLevel);
        const returnedFuelLevel = parseInt(document.getElementById('ReturnedFuelLevel').value) || 0;
        let fuelCharge = 0;
        if (returnedFuelLevel < initialFuelLevel) {
            fuelCharge = (initialFuelLevel - returnedFuelLevel) * 150;
        }
        document.getElementById('fuelCharge').textContent = fuelCharge.toFixed(0);

        const damageCharge = parseFloat(document.getElementById('DamageCharge').value) || 0;
        document.getElementById('damageCharge').textContent = damageCharge.toFixed(0);

        let penaltiesTotal = 0;
        document.querySelectorAll('input[name$=".Amount"]').forEach(input => {
            penaltiesTotal += parseFloat(input.value) || 0;
        });
        document.getElementById('penaltiesTotal').textContent = penaltiesTotal.toFixed(0);

        const baseAmount = @Model.TotalAmount;
        const finalAmount = baseAmount + delayCharge + fuelCharge + damageCharge + penaltiesTotal;
        document.getElementById('finalAmount').textContent = finalAmount.toFixed(0);

        const amountPaid = @Model.AmountPaid + (parseFloat(document.getElementById('AdditionalPayment').value) || 0);
        const remainingAmount = finalAmount - amountPaid;
        document.getElementById('remainingAmount').textContent = remainingAmount.toFixed(0);
    }

    // Show modal with message
    function showModal(title, message, isSuccess) {
        const modal = document.getElementById('submissionModal');
        const modalTitle = modal.querySelector('.modal-title');
        const modalBody = modal.querySelector('.modal-body');
        const viewDetailsBtn = modal.querySelector('#viewDetailsBtn');

        modalTitle.textContent = title;
        modalBody.textContent = message;
        modalTitle.className = `modal-title ${isSuccess ? 'text-success' : 'text-danger'}`;
        viewDetailsBtn.classList.toggle('d-none', !isSuccess);

        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
    }

    // Initialize page
    document.addEventListener('DOMContentLoaded', () => {
        // Bind calculation to input changes
        ['ReturnDate', 'ReturnedFuelLevel', 'DamageCharge', 'AdditionalPayment'].forEach(id => {
            document.getElementById(id).addEventListener('input', calculateFinalAmount);
        });
        document.querySelectorAll('input[name$=".Amount"]').forEach(input => {
            input.addEventListener('input', calculateFinalAmount);
        });
        calculateFinalAmount();

        // Handle form submission
        document.getElementById('checkInForm').addEventListener('submit', function (e) {
            e.preventDefault();
            const $btn = document.getElementById('submitCheckInBtn');
            $btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> جاري الإنهاء...';
            $btn.disabled = true;

            const formData = new FormData(this);
            fetch(this.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            })
                .then(response => response.json())
                .then(data => {
                    $btn.innerHTML = '<i class="fas fa-check-circle me-2"></i> إنهاء الحجز وطباعة الفاتورة';
                    $btn.disabled = false;
                    if (data.success) {
                        showModal('نجاح', 'تم إنهاء الحجز بنجاح. يمكنك عرض التفاصيل أو طباعة الفاتورة.', true);
                    } else {
                        showModal('خطأ', data.message || 'فشل إنهاء الحجز. يرجى المحاولة مرة أخرى.', false);
                    }
                })
                .catch(error => {
                    $btn.innerHTML = '<i class="fas fa-check-circle me-2"></i> إنهاء الحجز وطباعة الفاتورة';
                    $btn.disabled = false;
                    showModal('خطأ', 'حدث خطأ أثناء إنهاء الحجز: ' + error.message, false);
                });
        });
    });

    // Expose functions to global scope
    window.addPenalty = addPenalty;
    window.removePenalty = removePenalty;
})();