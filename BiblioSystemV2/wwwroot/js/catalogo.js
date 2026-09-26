(() => {
    const tree = document.getElementById("arbolCategorias");
    if (!tree) {
        return;
    }

    const folderCount = document.getElementById("cantidadLibrosCarpeta");
    const folderTitle = document.getElementById("categoriaSeleccionada");
    const bookDetails = document.getElementById("libroSeleccionado");
    const totalBooks = document.getElementById("cantidadLibros");
    const totalCategories = document.getElementById("cantidadCategorias");
    const categorySelect = document.getElementById("categoriaLibro");
    const graphCategorySelect = document.getElementById("graphCategorySelect");
    const graphBooksSelect = document.getElementById("graphBooksSelect");
    const graphOutput = document.getElementById("graphvizOutput");
    let uploadedXmlFile = null;
    let graphObjectUrl = null;
    let catalogRoot = null;

    function clearCategoryOptions() {
        [categorySelect, graphCategorySelect, graphBooksSelect].forEach((select) => {
            const placeholder = document.createElement("option");
            placeholder.value = "";
            placeholder.textContent = "Seleccionar categoría";
            select.replaceChildren(placeholder);
        });
    }

    function addCategoryOptions(category, depth = 0) {
        const selectors = category.fromXml === false
            ? [categorySelect]
            : [categorySelect, graphCategorySelect, graphBooksSelect];
        selectors.forEach((select) => {
            const option = document.createElement("option");
            option.value = category.nombre;
            option.textContent = `${"- ".repeat(depth)}${category.nombre}`;
            select.append(option);
        });

        category.subcategorias.forEach((child) => addCategoryOptions(child, depth + 1));
    }

    function clearCatalog(message, isError = false) {
        catalogRoot = null;
        totalBooks.textContent = "0";
        totalCategories.textContent = "0";
        document.getElementById("menorIsbn").textContent = "--";
        document.getElementById("mayorIsbn").textContent = "--";
        folderTitle.textContent = "Sin catálogo";
        folderCount.textContent = "Carga un XML para iniciar";
        uploadedXmlFile = null;
        clearCategoryOptions();
        if (graphObjectUrl) {
            URL.revokeObjectURL(graphObjectUrl);
            graphObjectUrl = null;
        }
        graphOutput.replaceChildren();
        const graphMessage = document.createElement("p");
        graphMessage.className = "browser-message";
        graphMessage.textContent = "Carga un XML para generar gráficos.";
        graphOutput.append(graphMessage);
        tree.replaceChildren();
        const text = document.createElement("p");
        text.className = isError ? "browser-message error" : "browser-message";
        text.textContent = message;
        tree.append(text);

        bookDetails.replaceChildren();
        const empty = document.createElement("p");
        empty.textContent = "No hay un catálogo cargado.";
        bookDetails.append(empty);
    }

    function countBooks(category) {
        return category.libros.length + category.subcategorias.reduce(
            (count, child) => count + countBooks(child), 0);
    }

    function countCategories(category) {
        return 1 + category.subcategorias.reduce(
            (count, child) => count + countCategories(child), 0);
    }

    function visitCategories(category, action) {
        action(category);
        category.subcategorias.forEach((child) => visitCategories(child, action));
    }

    function createBookNode(book) {
        return { book, left: null, right: null };
    }

    function insertBookNode(root, book) {
        if (!root) {
            return { root: createBookNode(book), inserted: true };
        }

        if (book.isbn === root.book.isbn) {
            return { root, inserted: false };
        }

        const side = book.isbn < root.book.isbn ? "left" : "right";
        const result = insertBookNode(root[side], book);
        root[side] = result.root;
        return { root, inserted: result.inserted };
    }

    function searchBookNode(root, isbn) {
        let current = root;
        while (current) {
            if (current.book.isbn === isbn) {
                return current.book;
            }
            current = isbn < current.book.isbn ? current.left : current.right;
        }
        return null;
    }

    function removeBookNode(root, isbn) {
        if (!root) {
            return { root: null, removed: null };
        }

        if (isbn < root.book.isbn) {
            const result = removeBookNode(root.left, isbn);
            root.left = result.root;
            return { root, removed: result.removed };
        }
        if (isbn > root.book.isbn) {
            const result = removeBookNode(root.right, isbn);
            root.right = result.root;
            return { root, removed: result.removed };
        }

        if (!root.left) {
            return { root: root.right, removed: root.book };
        }
        if (!root.right) {
            return { root: root.left, removed: root.book };
        }

        let successorParent = root;
        let successor = root.right;
        while (successor.left) {
            successorParent = successor;
            successor = successor.left;
        }

        root.book = successor.book;
        if (successorParent === root) {
            successorParent.right = successor.right;
        } else {
            successorParent.left = successor.right;
        }
        return { root, removed: { isbn, title: "", author: "", category: "" } };
    }

    function listBooksInOrder(root, books = []) {
        if (!root) {
            return books;
        }
        listBooksInOrder(root.left, books);
        books.push(root.book);
        listBooksInOrder(root.right, books);
        return books;
    }

    function initializeBookTrees(category) {
        let root = null;
        category.libros.forEach((book) => {
            root = insertBookNode(root, book).root;
        });
        category.bookTree = root;
        category.subcategorias.forEach(initializeBookTrees);
    }

    function synchronizeBookLists(category) {
        category.libros = listBooksInOrder(category.bookTree);
        category.subcategorias.forEach(synchronizeBookLists);
    }

    function findBook(isbn) {
        let found = null;
        visitCategories(catalogRoot, (category) => {
            if (!found) {
                const book = searchBookNode(category.bookTree, isbn);
                if (book) {
                    found = { book, category };
                }
            }
        });
        return found;
    }

    function refreshBookSummary() {
        let minimum = null;
        let maximum = null;
        let count = 0;
        visitCategories(catalogRoot, (category) => {
            const books = listBooksInOrder(category.bookTree);
            count += books.length;
            if (books.length === 0) {
                return;
            }
            minimum = minimum == null ? books[0].isbn : Math.min(minimum, books[0].isbn);
            const lastIsbn = books[books.length - 1].isbn;
            maximum = maximum == null ? lastIsbn : Math.max(maximum, lastIsbn);
        });

        totalBooks.textContent = String(count);
        document.getElementById("menorIsbn").textContent = minimum == null ? "--" : String(minimum);
        document.getElementById("mayorIsbn").textContent = maximum == null ? "--" : String(maximum);
    }

    function showSearchMessage(message, isError = false) {
        const result = document.getElementById("resultadoBusqueda");
        result.replaceChildren();
        const text = document.createElement("p");
        text.className = isError ? "browser-message error" : "browser-message";
        text.textContent = message;
        result.append(text);
    }

    function parseIsbn(inputId) {
        const value = document.getElementById(inputId).value.trim();
        if (!/^\d+$/.test(value) || Number(value) <= 0) {
            return null;
        }
        return Number(value);
    }

    function showOperationMessage(message, isError = false) {
        let status = document.getElementById("catalogOperationStatus");
        if (!status) {
            status = document.createElement("p");
            status.id = "catalogOperationStatus";
            status.className = "mutation-status";
            document.querySelector(".forms-grid").after(status);
        }
        status.textContent = message;
        status.classList.toggle("error", isError);
    }

    function refreshCatalogView() {
        synchronizeBookLists(catalogRoot);
        renderCatalog(catalogRoot);
        refreshBookSummary();
    }

    window.mostrarFormularioCategoria = () => {
        if (!catalogRoot) {
            showOperationMessage("Primero carga un archivo XML para agregar una categoría.", true);
            return;
        }

        const nombre = window.prompt("Nombre de la nueva categoría de Catalogo:");
        if (nombre == null) {
            return;
        }

        const nombreLimpio = nombre.trim();
        if (!nombreLimpio) {
            showOperationMessage("El nombre de la categoría no puede quedar vacío.", true);
            return;
        }

        let duplicada = false;
        visitCategories(catalogRoot, (category) => {
            if (category.nombre.localeCompare(nombreLimpio, undefined, { sensitivity: "accent" }) === 0) {
                duplicada = true;
            }
        });
        if (duplicada) {
            showOperationMessage(`Ya existe una categoría llamada '${nombreLimpio}'.`, true);
            return;
        }

        synchronizeBookLists(catalogRoot);
        catalogRoot.subcategorias.push({
            nombre: nombreLimpio,
            libros: [],
            subcategorias: [],
            fromXml: false
        });
        catalogRoot.subcategorias.sort((left, right) => left.nombre.localeCompare(right.nombre));
        renderCatalog(catalogRoot);
        refreshBookSummary();
        showOperationMessage(`Categoría '${nombreLimpio}' agregada temporalmente a Catalogo. El XML original no se modificó.`);
    };

    window.registrarLibro = () => {
        if (!catalogRoot) {
            showOperationMessage("Primero carga un archivo XML.", true);
            return;
        }

        const isbn = parseIsbn("isbn");
        const title = document.getElementById("titulo").value.trim();
        const author = document.getElementById("autor").value.trim();
        const categoryName = categorySelect.value;
        if (isbn == null || !title || !author || !categoryName) {
            showOperationMessage("Completa ISBN, título, autor y categoría con datos válidos.", true);
            return;
        }
        if (findBook(isbn)) {
            showOperationMessage(`Ya existe un libro con el ISBN ${isbn}.`, true);
            return;
        }

        let targetCategory = null;
        visitCategories(catalogRoot, (category) => {
            if (category.nombre === categoryName) {
                targetCategory = category;
            }
        });
        if (!targetCategory) {
            showOperationMessage("No se encontró la categoría seleccionada.", true);
            return;
        }

        const book = { isbn, titulo: title, autor: author, categoria: categoryName };
        const result = insertBookNode(targetCategory.bookTree, book);
        targetCategory.bookTree = result.root;
        refreshCatalogView();
        document.getElementById("isbn").value = "";
        document.getElementById("titulo").value = "";
        document.getElementById("autor").value = "";
        categorySelect.value = "";
        showOperationMessage(`Libro agregado temporalmente. El XML original no se modificó.`);
    };

    window.buscarLibro = () => {
        if (!catalogRoot) {
            showSearchMessage("Primero carga un archivo XML.", true);
            return;
        }

        const isbn = parseIsbn("buscarIsbn");
        if (isbn == null) {
            showSearchMessage("Ingresa un ISBN positivo y válido.", true);
            return;
        }

        const match = findBook(isbn);
        if (!match) {
            showSearchMessage(`No se encontró un libro con ISBN ${isbn}.`, true);
            return;
        }

        showBook(match.book);
        const result = document.getElementById("resultadoBusqueda");
        result.replaceChildren();
        const text = document.createElement("p");
        text.textContent = `${match.book.titulo} | ${match.book.autor} | ${match.category.nombre}`;
        result.append(text);
    };

    window.eliminarLibro = () => {
        if (!catalogRoot) {
            showOperationMessage("Primero carga un archivo XML.", true);
            return;
        }

        const isbn = parseIsbn("eliminarIsbn");
        if (isbn == null) {
            showOperationMessage("Ingresa un ISBN positivo y válido.", true);
            return;
        }

        const match = findBook(isbn);
        if (!match) {
            showOperationMessage(`No se encontró un libro con ISBN ${isbn}.`, true);
            return;
        }

        const result = removeBookNode(match.category.bookTree, isbn);
        match.category.bookTree = result.root;
        refreshCatalogView();
        document.getElementById("eliminarIsbn").value = "";
        showSearchMessage("Ingresa un ISBN para realizar una búsqueda.");
        showOperationMessage(`Libro ${isbn} eliminado temporalmente. El XML original no se modificó.`);
    };

    function updateFolderSummary(category) {
        folderTitle.textContent = category.nombre;
        const count = category.libros.length;
        folderCount.textContent = `${count} ${count === 1 ? "libro" : "libros"} en esta carpeta`;
    }

    function showBook(book) {
        bookDetails.replaceChildren();

        const title = document.createElement("h4");
        title.textContent = book.titulo;
        bookDetails.append(title);

        const details = document.createElement("dl");
        [
            ["ISBN", book.isbn],
            ["Autor", book.autor],
            ["Categoría", book.categoria]
        ].forEach(([label, value]) => {
            const item = document.createElement("div");
            item.className = "book-detail";

            const term = document.createElement("dt");
            term.textContent = label;
            const description = document.createElement("dd");
            description.textContent = String(value);

            item.append(term, description);
            details.append(item);
        });

        bookDetails.append(details);
    }

    function createBookButton(book) {
        const button = document.createElement("button");
        button.type = "button";
        button.className = "book-button";
        button.textContent = book.titulo;
        button.addEventListener("click", () => {
            tree.querySelectorAll(".book-button.is-selected").forEach((selected) => {
                selected.classList.remove("is-selected");
                selected.setAttribute("aria-pressed", "false");
            });
            button.classList.add("is-selected");
            button.setAttribute("aria-pressed", "true");
            showBook(book);
        });
        button.setAttribute("aria-pressed", "false");
        return button;
    }

    function createFolder(category, expanded = false) {
        const folder = document.createElement("div");
        folder.className = "folder-node";

        const button = document.createElement("button");
        button.type = "button";
        button.className = "folder-button";
        button.setAttribute("aria-expanded", String(expanded));

        const indicator = document.createElement("span");
        indicator.className = "folder-indicator";
        indicator.setAttribute("aria-hidden", "true");
        indicator.textContent = expanded ? "v" : ">";

        const name = document.createElement("span");
        name.className = "folder-name";
        name.textContent = category.nombre;

        const badge = document.createElement("span");
        badge.className = "folder-badge";
        badge.textContent = String(category.libros.length);
        badge.setAttribute("aria-label", `${category.libros.length} libros`);

        button.append(indicator, name, badge);

        const contents = document.createElement("div");
        contents.className = "folder-contents";
        contents.hidden = !expanded;

        category.subcategorias.forEach((child) => contents.append(createFolder(child, true)));

        if (category.libros.length > 0) {
            const bookToggle = document.createElement("button");
            bookToggle.type = "button";
            bookToggle.className = "book-list-toggle";
            bookToggle.setAttribute("aria-expanded", "false");
            bookToggle.textContent = `Mostrar libros (${category.libros.length})`;

            const books = document.createElement("div");
            books.className = "folder-books";
            books.hidden = true;
            category.libros.forEach((book) => books.append(createBookButton(book)));

            bookToggle.addEventListener("click", () => {
                const open = books.hidden;
                books.hidden = !open;
                bookToggle.setAttribute("aria-expanded", String(open));
                bookToggle.textContent = `${open ? "Ocultar" : "Mostrar"} libros (${category.libros.length})`;
            });

            contents.append(bookToggle, books);
        } else if (category.subcategorias.length === 0) {
            const empty = document.createElement("p");
            empty.className = "empty-folder";
            empty.textContent = "Esta carpeta no contiene libros.";
            contents.append(empty);
        }

        button.addEventListener("click", () => {
            const open = contents.hidden;
            contents.hidden = !open;
            button.setAttribute("aria-expanded", String(open));
            indicator.textContent = open ? "v" : ">";
            folder.classList.toggle("is-open", open);
            updateFolderSummary(category);
        });

        folder.append(button, contents);
        folder.classList.toggle("is-open", expanded);
        return folder;
    }

    function renderCatalog(rootCategory) {
        catalogRoot = rootCategory;
        initializeBookTrees(catalogRoot);
        tree.replaceChildren(createFolder(rootCategory, true));
        clearCategoryOptions();
        addCategoryOptions(rootCategory);
        totalBooks.textContent = String(countBooks(rootCategory));
        totalCategories.textContent = String(countCategories(rootCategory));
        refreshBookSummary();
        updateFolderSummary(rootCategory);
        bookDetails.replaceChildren();
        const message = document.createElement("p");
        message.textContent = "Selecciona un libro de una carpeta para consultar su información.";
        bookDetails.append(message);
    }

    async function generateGraph(type, categoryName) {
        if (!uploadedXmlFile) {
            graphOutput.textContent = "Primero carga un archivo XML.";
            return;
        }
        if (!categoryName) {
            graphOutput.textContent = "Selecciona una categoría para el gráfico.";
            return;
        }

        graphOutput.textContent = "Generando gráfico...";
        try {
            const datos = new FormData();
            datos.append("archivo", uploadedXmlFile);
            datos.append("tipo", type);
            datos.append("categoria", categoryName);

            const response = await fetch(tree.dataset.graphUrl, {
                method: "POST",
                body: datos
            });
            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.mensaje || "No fue posible generar el gráfico.");
            }

            const svg = await response.blob();
            if (graphObjectUrl) {
                URL.revokeObjectURL(graphObjectUrl);
            }
            graphObjectUrl = URL.createObjectURL(svg);

            const image = document.createElement("img");
            image.className = "graphviz-image";
            image.src = graphObjectUrl;
            image.alt = type === "categorias"
                ? `Árbol de categorías desde ${categoryName}`
                : `Árbol de libros de ${categoryName}`;

            const download = document.createElement("a");
            download.className = "graphviz-download";
            download.href = graphObjectUrl;
            download.download = `arbol-${type}-${categoryName}.svg`;
            download.textContent = "Descargar SVG";

            graphOutput.replaceChildren(image, download);
        } catch (error) {
            graphOutput.textContent = error.message || "No fue posible generar el gráfico.";
        }
    }

    document.querySelectorAll("[data-graph-type]").forEach((button) => {
        button.addEventListener("click", () => {
            const select = document.getElementById(button.dataset.categorySelect);
            generateGraph(button.dataset.graphType, select.value);
        });
    });

    window.cargarXML = async (input) => {
        const file = input.files?.[0];
        if (!file) {
            return;
        }

        clearCatalog("Enviando el archivo al servicio XML...");
        try {
            const datos = new FormData();
            datos.append("archivo", file);

            const response = await fetch(tree.dataset.uploadUrl, {
                method: "POST",
                body: datos
            });
            const resultado = await response.json().catch(() => ({}));
            if (!response.ok) {
                throw new Error(resultado.mensaje || "El servicio no pudo leer el archivo XML.");
            }

            uploadedXmlFile = file;
            visitCategories(resultado, (category) => {
                category.fromXml = true;
            });
            renderCatalog(resultado);
            showOperationMessage("Catálogo cargado. Agregar y eliminar libros solo cambia el árbol temporal de esta página.");
        } catch (error) {
            clearCatalog(error.message || "No fue posible cargar el archivo XML.", true);
        } finally {
            input.value = "";
        }
    };

    clearCatalog("Selecciona un archivo XML para cargar el catálogo.");
})();