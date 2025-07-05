/**
 * ملف JavaScript لصفحة تأجير السيارات السريع - تصميم محسّن
 */
(function () {
    const { showSuccessMessage, showErrorMessage, validateField, clearFieldError } = window.validation;

    let selectedCarData = null;
    let formValidation = {
        customer: false,
        car: false,
        dates: false,
        fuel: false,
        odometer: false,
        paid: true
    };

    // تهيئة الصفحة
    document.addEventListener("DOMContentLoaded", () => {
        console.log("Quick.js: Page loaded, initializing...");
        initializePage();
        setupEventListeners();
        setupFormValidation();
        initializeSelect2();
    });

    function initializePage() {
        setDefaultDates();
        updateFormValidation();
    }

    function initializeSelect2() {
        $("#customerSelect").select2({
            placeholder: "-- اختر عميل --",
            allowClear: true,
            dir: "rtl",
            minimumResultsForSearch: 0, // إزالة الحد الأدنى للسماح بفتح القائمة مباشرة
            ajax: {
                url: "/Rental/GetCustomers",
                dataType: "json",
                delay: 250,
                data: params => ({ search: params.term || "" }), // إرسال بحث فارغ لجلب جميع العملاء
                processResults: data => ({
                    results: data.map(c => ({
                        id: c.id,
                        text: `${c.fullName} (${c.phoneNumber})${c.hasPreviousRental ? ' (حجز سابق)' : ''}`
                    }))
                }),
                cache: true // تفعيل التخزين المؤقت لتحسين الأداء
            }
        }).on("change", handleCustomerSelection);

        // تحميل العملاء مسبقًا عند فتح القائمة
        fetch("/Rental/GetCustomers", {
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            }
        })
            .then(response => response.json())
            .then(data => {
                const $select = $("#customerSelect");
                data.forEach(customer => {
                    const option = new Option(
                        `${customer.fullName} (${customer.phoneNumber})${customer.hasPreviousRental ? ' (حجز سابق)' : ''}`,
                        customer.id,
                        false,
                        false
                    );
                    $select.append(option);
                });
                $select.trigger("change"); // تحديث Select2 بعد إضافة الخيارات
            })
            .catch(error => {
                showErrorMessage("فشل تحميل قائمة العملاء: " + error.message);
            });
    }

    function setupEventListeners() {
        document.querySelectorAll("input[name='StartDate'], input[name='EndDate']").forEach(input => {
            input.addEventListener("change", handleDateChange);
        });

        document.getElementById("paidAmount").addEventListener("input", handlePaidAmount);
        document.getElementById("paidAmount").addEventListener("blur", formatCurrencyInput);
        document.getElementById("openCarModal").addEventListener("click", loadCarModal);
        document.getElementById("openCustomerModal").addEventListener("click", loadCustomerModal);

        document.getElementById("submitBtn").addEventListener("click", (e) => {
            e.preventDefault();
            showConfirmModal();
        });

        const confirmBtn = document.getElementById("confirmSubmitBtn");
        if (confirmBtn) {
            confirmBtn.addEventListener("click", handleFormSubmit);
            console.log("Event listener added to confirm button");
        } else {
            console.error("Confirm button not found!");
        }
    }

    function setupFormValidation() {
        $("#rentalForm").validate({
            rules: {
                CustomerId: "required",
                CarId: "required",
                StartDate: "required",
                EndDate: { required: true, greaterThan: "#StartDate" },
                InitialFuelLevel: "required",
                InitialOdometer: { required: true, min: 0 },
                PaidAmount: { min: 0 }
            },
            messages: {
                CustomerId: "يجب اختيار عميل",
                CarId: "يجب اختيار سيارة",
                StartDate: "تاريخ الاستلام مطلوب",
                EndDate: "تاريخ التسليم مطلوب ويجب أن يكون بعد الاستلام",
                InitialFuelLevel: "مستوى البنزين مطلوب",
                InitialOdometer: "قراءة العداد يجب أن تكون 0 أو أكبر",
                PaidAmount: "المبلغ يجب أن يكون 0 أو أكبر"
            },
            errorElement: "span",
            errorClass: "text-danger",
            errorPlacement: (error, element) => error.insertAfter(element),
            highlight: element => $(element).addClass("is-invalid"),
            unhighlight: element => $(element).removeClass("is-invalid"),
            submitHandler: () => showConfirmModal()
        });
    }

    function setDefaultDates() {
        const now = new Date();
        const startInput = document.querySelector("input[name='StartDate']");
        const endInput = document.querySelector("input[name='EndDate']");
        if (startInput && !startInput.value) startInput.value = now.toISOString().slice(0, 16);
        if (endInput && !endInput.value) {
            const tomorrow = new Date(now);
            tomorrow.setDate(tomorrow.getDate() + 1);
            endInput.value = tomorrow.toISOString().slice(0, 16);
        }
        handleDateChange();
    }

    function handleCustomerSelection() {
        formValidation.customer = !!this.value;
        document.getElementById("selectedCustomerId").value = this.value;
        updateFormValidation();
    }

    function handleDateChange() {
        const startDate = new Date(document.querySelector("input[name='StartDate']").value);
        const endDate = new Date(document.querySelector("input[name='EndDate']").value);
        formValidation.dates = endDate > startDate;
        if (!formValidation.dates) showErrorMessage("تاريخ التسليم يجب أن يكون بعد تاريخ الاستلام");
        calculateTotal();
        updateFormValidation();
    }

    function handlePaidAmount() {
        const paid = parseFloat(this.value) || 0;
        formValidation.paid = paid >= 0;
        if (!formValidation.paid) document.querySelector(".paid-amount-error").style.display = "block";
        else document.querySelector(".paid-amount-error").style.display = "none";
        calculateTotal();
        updateFormValidation();
    }

    function loadCustomerModal() {
        const modalBody = document.getElementById("customerModalBody");
        modalBody.innerHTML = `
            <form id="addCustomerForm" class="needs-validation" novalidate>
                <div class="mb-3">
                    <label class="form-label">الاسم الكامل</label>
                    <input type="text" name="NewCustomerName" class="form-control" required>
                </div>
                <div class="mb-3">
                    <label class="form-label">رقم الهاتف</label>
                    <input type="text" name="NewCustomerPhone" class="form-control" required>
                </div>
                <div class="mb-3">
                    <label class="form-label">الرقم القومي</label>
                    <input type="text" name="NewCustomerNationalId" class="form-control" required>
                </div>
                <div class="mb-3">
                    <label class="form-label">رقم الرخصة</label>
                    <input type="text" name="NewCustomerLicense" class="form-control" required>
                </div>
                <div class="mb-3">
                    <label class="form-label">تاريخ انتهاء الرخصة</label>
                    <input type="date" name="NewCustomerLicenseExpiry" class="form-control" required
                           value="${new Date().toISOString().split('T')[0]}">
                </div>
                <div class="invalid-feedback" style="display:none;">يرجى ملء جميع الحقول</div>
            </form>
        `;

        const modalFooter = document.querySelector("#customerModal .modal-footer");
        modalFooter.innerHTML = `
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">إلغاء</button>
            <button type="button" class="btn btn-primary" id="saveCustomerBtn">حفظ</button>
        `;

        const modal = new bootstrap.Modal(document.getElementById("customerModal"));
        modal.show();

        document.getElementById("saveCustomerBtn").addEventListener("click", saveCustomer);
    }

    async function loadCarModal() {
        const modalBody = document.getElementById("carModalBody");
        try {
            const response = await fetch("/Rental/GetAvailableCars", {
                headers: {
                    "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                }
            });

            const cars = await response.json();
            modalBody.innerHTML = `
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th>رقم اللوحة</th>
                            <th>الماركة</th>
                            <th>الموديل</th>
                            <th>السعر/اليوم</th>
                            <th>إجراء</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${cars.length === 0 ? '<tr><td colspan="5" class="text-center">لا توجد سيارات متاحة حاليًا</td></tr>' :
                    cars.map(car => `
                            <tr>
                                <td>${car.plateNumber || 'N/A'}</td>
                                <td>${car.brand || 'N/A'}</td>
                                <td>${car.model || 'N/A'}</td>
                                <td>${(car.dailyRate || 0).toLocaleString('ar-EG')} جنيه</td>
                                <td>
                                    <button class="btn btn-primary btn-select-car" 
                                            data-id="${car.id}"
                                            data-display="${car.brand} ${car.model} (${car.plateNumber})"
                                            data-dailyrate="${car.dailyRate || 0}">
                                        اختر
                                    </button>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            `;

            const modal = new bootstrap.Modal(document.getElementById("carModal"));
            modal.show();
            setupCarSelectionEvents();
        } catch (error) {
            showErrorMessage("فشل تحميل قائمة السيارات: " + error.message);
        }
    }

    function setupCarSelectionEvents() {
        document.querySelectorAll(".btn-select-car").forEach(button => {
            button.addEventListener("click", function () {
                const carId = this.dataset.id;
                const displayText = this.dataset.display;
                const dailyRate = parseFloat(this.dataset.dailyrate);
                selectCar(carId, displayText, dailyRate);
                bootstrap.Modal.getInstance(document.getElementById("carModal")).hide();
            });
        });
    }

    function selectCar(carId, displayText, dailyRate) {
        selectedCarData = { id: carId, displayText, dailyRate };
        document.getElementById("selectedCarId").value = carId;
        document.getElementById("selectedCarInfo").innerHTML = `
            <i class="fas fa-car text-success me-2"></i>
            <strong>${displayText}</strong> 
            (<span>${dailyRate.toLocaleString('ar-EG')} جنيه/يوم</span>)
        `;
        formValidation.car = true;
        calculateTotal();
        showSuccessMessage(`تم اختيار السيارة: ${displayText}`);
        updateFormValidation();
    }

    function calculateTotal() {
        if (!selectedCarData || !formValidation.dates) return;

        const startDate = new Date(document.querySelector("input[name='StartDate']").value);
        const endDate = new Date(document.querySelector("input[name='EndDate']").value);
        const days = Math.max(1, Math.ceil((endDate - startDate) / (1000 * 3600 * 24)));
        const total = days * selectedCarData.dailyRate;
        const paid = parseFloat(document.getElementById("paidAmount").value) || 0;
        updatePriceDisplay(selectedCarData.dailyRate, days, total, total - paid);
    }

    function updatePriceDisplay(dailyRate, days, total, remaining) {
        document.getElementById("dailyRate").textContent = dailyRate.toLocaleString("ar-EG");
        document.getElementById("daysCount").textContent = days;
        document.getElementById("totalAmount").textContent = total.toLocaleString("ar-EG");
        document.getElementById("remainingAmount").textContent = remaining.toLocaleString("ar-EG");
        document.getElementById("remainingAmount").className = remaining > 0 ? "text-warning" : "text-success";
    }

    function formatCurrencyInput() {
        const value = parseFloat(this.value) || 0;
        this.value = value;
        handlePaidAmount.call(this);
    }

    async function saveCustomer() {
        const form = document.getElementById("addCustomerForm");
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        const data = {
            NewCustomerName: form.querySelector('[name="NewCustomerName"]').value,
            NewCustomerPhone: form.querySelector('[name="NewCustomerPhone"]').value,
            NewCustomerNationalId: form.querySelector('[name="NewCustomerNationalId"]').value,
            NewCustomerLicense: form.querySelector('[name="NewCustomerLicense"]').value,
            NewCustomerLicenseExpiry: form.querySelector('[name="NewCustomerLicenseExpiry"]').value
        };

        try {
            const response = await fetch("/Rental/AddCustomer", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            if (result.success) {
                addCustomerToSelect({
                    id: result.customerId,
                    text: result.text
                });
                bootstrap.Modal.getInstance(document.getElementById("customerModal")).hide();
                showSuccessMessage("تم إضافة العميل بنجاح");
                form.reset();
            } else {
                showErrorMessage(result.message || "فشل إضافة العميل");
            }
        } catch (error) {
            showErrorMessage("حدث خطأ أثناء حفظ العميل: " + error.message);
        }
    }

    function addCustomerToSelect(customer) {
        const $select = $("#customerSelect");
        $select.append(new Option(customer.text, customer.id, true, true)).trigger("change");
        formValidation.customer = true;
        updateFormValidation();
    }

    function showConfirmModal() {
        if (!validateForm()) {
            showErrorMessage("يرجى ملء جميع الحقول المطلوبة بشكل صحيح");
            return;
        }

        const customerName = $("#customerSelect option:selected").text() || "غير محدد";
        const carName = document.querySelector("#selectedCarInfo strong")?.textContent || "غير محدد";
        const startDate = new Date(document.querySelector("input[name='StartDate']").value).toLocaleString("ar-EG");
        const endDate = new Date(document.querySelector("input[name='EndDate']").value).toLocaleString("ar-EG");
        const days = document.getElementById("daysCount").textContent;
        const total = document.getElementById("totalAmount").textContent;
        const paid = document.getElementById("paidAmount").value || "0";
        const remaining = document.getElementById("remainingAmount").textContent;

        document.getElementById("confirmCustomer").textContent = customerName;
        document.getElementById("confirmCar").textContent = carName;
        document.getElementById("confirmStartDate").textContent = startDate;
        document.getElementById("confirmEndDate").textContent = endDate;
        document.getElementById("confirmDaysCount").textContent = days;
        document.getElementById("confirmTotalAmount").textContent = `${total} جنيه`;
        document.getElementById("confirmPaidAmount").textContent = `${parseFloat(paid).toLocaleString("ar-EG")} جنيه`;
        document.getElementById("confirmRemainingAmount").textContent = `${remaining} جنيه`;

        const modal = new bootstrap.Modal(document.getElementById("confirmRentalModal"));
        modal.show();
    }

    function handleFormSubmit() {
        const confirmBtn = document.getElementById("confirmSubmitBtn");
        confirmBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i> جاري الحفظ...';
        confirmBtn.disabled = true;

        if (!validateForm()) {
            showErrorMessage("يرجى ملء جميع الحقول المطلوبة بشكل صحيح");
            resetConfirmButton(confirmBtn);
            return;
        }

        const modal = bootstrap.Modal.getInstance(document.getElementById("confirmRentalModal"));
        if (modal) modal.hide();

        const form = document.getElementById("rentalForm");
        if (form) {
            form.submit();
        } else {
            console.error("Quick.js: rentalForm not found");
            showErrorMessage("خطأ في النموذج: النموذج غير موجود");
            resetConfirmButton(confirmBtn);
        }
    }

    function resetConfirmButton(confirmBtn) {
        setTimeout(() => {
            confirmBtn.innerHTML = '<i class="fas fa-check-circle me-1"></i> تأكيد الحجز';
            confirmBtn.disabled = false;
        }, 1000);
    }

    function validateForm() {
        let isValid = true;
        const requiredFields = [
            { selector: "#selectedCustomerId", name: "CustomerId", error: ".customer-error" },
            { selector: "#selectedCarId", name: "CarId", error: ".car-error" },
            { selector: "input[name='StartDate']", name: "StartDate", error: ".date-error" },
            { selector: "input[name='EndDate']", name: "EndDate", error: ".date-error" },
            { selector: "select[name='InitialFuelLevel']", name: "InitialFuelLevel", error: ".fuel-error" },
            { selector: "input[name='InitialOdometer']", name: "InitialOdometer", error: ".odometer-error" }
        ];

        requiredFields.forEach(field => {
            const el = document.querySelector(field.selector);
            const errorEl = document.querySelector(field.error);

            if (!el || !el.value) {
                $(el).addClass("is-invalid").removeClass("is-valid");
                if (errorEl) {
                    errorEl.style.display = "block";
                    errorEl.textContent = "هذا الحقل مطلوب";
                }
                isValid = false;
            } else {
                $(el).removeClass("is-invalid").addClass("is-valid");
                if (errorEl) errorEl.style.display = "none";
            }
        });

        const startDate = new Date(document.querySelector("input[name='StartDate']").value);
        const endDate = new Date(document.querySelector("input[name='EndDate']").value);
        if (isValid && endDate <= startDate) {
            const errorEl = document.querySelector(".date-error");
            if (errorEl) {
                errorEl.textContent = "تاريخ التسليم يجب أن يكون بعد تاريخ الاستلام";
                errorEl.style.display = "block";
            }
            isValid = false;
        }

        return isValid;
    }

    function updateFormValidation() {
        const submitBtn = document.getElementById("submitBtn");
        const isValid = validateForm();
        if (submitBtn) {
            submitBtn.disabled = !isValid;
            submitBtn.classList.toggle("btn-primary", isValid);
            submitBtn.classList.toggle("btn-secondary", !isValid);
        }
    }
})();