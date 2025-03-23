// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//function liveSearch
window.liveSearch= function(searchInputId, itemSelector, dataAttribute) {
    const searchInput = document.getElementById(searchInputId);
    const items = document.querySelectorAll(itemSelector);

    function removeDiacritics(str) {
        return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
    }

    searchInput.addEventListener('input', function () {
        const query = removeDiacritics(this.value);

        items.forEach(function (item) {
            const name = removeDiacritics(item.getAttribute(dataAttribute));
            //   item.style.display = name.includes(query) ? 'flex' : 'none';
            item.classList.toggle('hidden', !name.includes(query)); // Používá 'hidden' pro skrytí


        });
    });
}


//window.moveCheckedIngredients = function (itemSelector, checkboxSelector) {
//    const items = document.querySelectorAll(itemSelector);

//    items.forEach(item => {
//        const checkbox = item.querySelector(checkboxSelector);
//        checkbox.addEventListener('change', function () {
//            const parent = item.parentNode;
//            // Pokud je zaškrtnuto, přesuneme položku na začátek
//            if (checkbox.checked) {
//                parent.prepend(item); // Přesune na začátek
//            } else {
//                parent.appendChild(item); // Pokud není zaškrtnuto, vložíme zpět na konec
//            }
//        });
//    });
//};


//FUnkční, ale DĚLÁ DÍRY
//window.moveCheckedIngredients = function (itemSelector, checkboxSelector) {
//    const items = document.querySelectorAll(itemSelector);
//    //const itemPositions = [];

//    items.forEach((item, index) => {
//        item.setAttribute('data-original-index', index);
//    });

//    items.forEach(item => {
//        const checkbox = item.querySelector(checkboxSelector);
//        checkbox.addEventListener('change', function () {
//            const parent = item.parentNode;
//            // Pokud je zaškrtnuto, přesuneme položku na začátek
//            if (checkbox.checked) {
//                parent.prepend(item); // Přesune na začátek
//            } else {
//                // Pokud není zaškrtnuto, vrátíme položku na její původní pozici
//                const originalIndex = item.getAttribute('data-original-index');

//                const allItems = Array.from(parent.children);  // Seznam všech položek
//                const currentIndex = allItems.indexOf(item);

//                if (currentIndex !== parseInt(originalIndex)) {
//                    // Pokud položka není na původním místě, přesuneme ji zpět
//                    parent.insertBefore(item, allItems[originalIndex]);
//                }

//            }
//        });
//    });
//};


//Skoro funguje
/*
window.moveCheckedIngredients = function (itemSelector, checkboxSelector) {
    const items = document.querySelectorAll(itemSelector);

    // Uchování původní pozice pro každou položku
    items.forEach((item, index) => {
        item.setAttribute('data-original-index', index);
    });

    // Funkce pro přesunutí zaškrtnutých položek na začátek
    function moveCheckedToTop() {
        const parent = items[0].parentNode; // Všechny položky mají stejného rodiče
        // Seřadíme všechny položky, aby zaškrtnuté byly na začátku
        const sortedItems = Array.from(items).sort((a, b) => {
            const aChecked = a.querySelector(checkboxSelector).checked;
            const bChecked = b.querySelector(checkboxSelector).checked;
            // Zaškrtnuté položky jdou na začátek
            return bChecked - aChecked;
        });

        // Znovu přidáme položky do rodiče v novém pořadí
        sortedItems.forEach(item => {
            parent.appendChild(item);
        });
    }

    // Přidání event listeneru na změnu checkboxu
    items.forEach(item => {
        const checkbox = item.querySelector(checkboxSelector);
        checkbox.addEventListener('change', function () {
            // Po každé změně stavu, zavoláme funkci pro správné seřazení
            moveCheckedToTop();
        });
    });

    //// Přidání event listeneru na políčka pro zadání množství
    //document.querySelectorAll('input[type="number"]').forEach(function (input) {
    //    input.addEventListener('blur', function () {
    //        moveCheckedToTop(); // Zavoláme přesunutí položek až po opuštění textového pole
    //    });
    //});

    // Zavoláme funkci na začátku pro správné seřazení při načtení stránky
    moveCheckedToTop();
};
*/


window.moveCheckedIngredients = function (itemSelector, checkboxSelector) {
    const items = document.querySelectorAll(itemSelector);

    // Uchování původní pozice pro každou položku
    items.forEach((item, index) => {
        if (!item.hasAttribute('data-original-index')) {
            item.setAttribute('data-original-index', index);
        }
    });

    // Funkce pro přesunutí zaškrtnutých položek na začátek
    function moveCheckedToTop() {
        const parent = items[0].parentNode; // Všechny položky mají stejného rodiče
        // Seřadíme všechny položky, aby zaškrtnuté byly na začátku
        const sortedItems = Array.from(items).sort((a, b) => {
            const aChecked = a.querySelector(checkboxSelector).checked;
            const bChecked = b.querySelector(checkboxSelector).checked;


            // Pokud oba zaškrtnuté nebo oba nezaškrtnuté → porovnáme původní pořadí
            if (aChecked === bChecked) {
                const aIndex = parseInt(a.getAttribute('data-original-index'));
                const bIndex = parseInt(b.getAttribute('data-original-index'));
                return aIndex - bIndex;
            }

            // Zaškrtnuté dopředu
            return bChecked - aChecked;


        });

        // Znovu přidáme položky do rodiče v novém pořadí
        sortedItems.forEach(item => {
            parent.appendChild(item);
        });
    }

    // Přidání event listeneru na změnu checkboxu
    items.forEach(item => {
        const checkbox = item.querySelector(checkboxSelector);
        checkbox.addEventListener('change', function () {
            // Po každé změně stavu, zavoláme funkci pro správné seřazení
            moveCheckedToTop();
        });
    });

    //// Přidání event listeneru na políčka pro zadání množství
    //document.querySelectorAll('input[type="number"]').forEach(function (input) {
    //    input.addEventListener('blur', function () {
    //        moveCheckedToTop(); // Zavoláme přesunutí položek až po opuštění textového pole
    //    });
    //});

    // Zavoláme funkci na začátku pro správné seřazení při načtení stránky
    moveCheckedToTop();
};



//Skoro funguje
// Automatické zaškrtnutí checkboxu při zadání množství
/*
window.autoCheckIngredients = function () {
    document.querySelectorAll('input[type="number"]').forEach(function (input) {
        input.addEventListener('input', function () {
            var checkbox = document.getElementById('ingredient-' + this.id.split('-')[1]);
            if (this.value && this.value > 0) {
                checkbox.checked = true;
            } else {
                checkbox.checked = false;
            }
            //window.moveCheckedIngredients('.ingredient-item', 'input[type="checkbox"]');
            // Přesun až po opuštění inputu
            input.addEventListener('blur', function () {
                window.moveCheckedIngredients('.ingredient-item', 'input[type="checkbox"]');
            });
        });
    });
};
*/

window.autoCheckIngredients = function () {
    document.querySelectorAll('input[type="number"]').forEach(function (input) {


        const ingredientId = input.id.split('-')[1];
        const checkbox = document.getElementById('ingredient-' + ingredientId);

        input.addEventListener('input', function () {
          //  var checkbox = document.getElementById('ingredient-' + this.id.split('-')[1]);
            if (this.value && this.value > 0) {
                checkbox.checked = true;
            } else {
                checkbox.checked = false;
            }
            //window.moveCheckedIngredients('.ingredient-item', 'input[type="checkbox"]');
            // Přesun až po opuštění inputu
            input.addEventListener('blur', function () {
                window.moveCheckedIngredients('.ingredient-item', 'input[type="checkbox"]');
            });
        });
    });
};

// Kontrola, zda je zaškrtnuto alespoň jedno z polí "recept je vhodný..."
window.validateCheckboxes = function () {
    const checkboxes = document.querySelectorAll('#suitableAsBreakfast, #suitableAsLunch, #suitableAsDinner, #suitableAsSnack');
    const submitButton = document.getElementById('submitButton');

    function updateSubmitButtonState() {
        let atLeastOneChecked = Array.from(checkboxes).some(cb => cb.checked);
        submitButton.disabled = !atLeastOneChecked;
    }

    checkboxes.forEach(cb => cb.addEventListener('change', updateSubmitButtonState));

    // Inicializace kontroly při načtení stránky
    window.addEventListener('DOMContentLoaded', updateSubmitButtonState);
};