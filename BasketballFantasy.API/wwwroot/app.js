const storageKey = "basketball-fantasy-session";

const state = {
    token: "",
    userName: "",
    role: "",
    currentView: "dashboard",
    search: "",
    resources: {},
    options: {},
    formState: null,
    detailState: null,
    dashboard: null,
    loading: false,
    toasts: []
};

const resources = {
    teams: {
        label: "Timovi",
        endpoint: "/teams",
        createLabel: "Novi tim",
        cardTitle: (item) => item.name,
        cardMeta: (item) => item.city,
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Naziv", key: "name" },
            { label: "Grad", key: "city" }
        ],
        formFields: [
            { name: "name", label: "Naziv", type: "text", required: true, placeholder: "Partizan" },
            { name: "city", label: "Grad", type: "text", required: true, placeholder: "Beograd" }
        ],
        extraActions: [
            { key: "players", label: "Igraci" }
        ]
    },
    players: {
        label: "Igraci",
        endpoint: "/players",
        createLabel: "Novi igrac",
        cardTitle: (item) => `${item.firstName} ${item.lastName}`,
        cardMeta: (item) => `${item.position} - ${item.teamName}`,
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Pozicija", key: "position" },
            { label: "Visina", value: (item) => `${item.height} cm` },
            { label: "Broj", key: "jerseyNumber" },
            { label: "Tim", key: "teamName" }
        ],
        formFields: [
            { name: "firstName", label: "Ime", type: "text", required: true },
            { name: "lastName", label: "Prezime", type: "text", required: true },
            { name: "position", label: "Pozicija", type: "text", required: true, placeholder: "PG / SG / SF / PF / C" },
            { name: "height", label: "Visina", type: "number", required: true, step: "0.01", min: 150 },
            { name: "jerseyNumber", label: "Broj na dresu", type: "number", required: true, min: 1 },
            { name: "teamId", label: "Tim", type: "select", required: true, optionsSource: "teams" }
        ],
        extraActions: [
            { key: "statistics", label: "Statistike" },
            { key: "totals", label: "Ukupno" }
        ]
    },
    coaches: {
        label: "Treneri",
        endpoint: "/coaches",
        createLabel: "Novi trener",
        cardTitle: (item) => `${item.firstName} ${item.lastName}`,
        cardMeta: (item) => item.teamName,
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Tim", key: "teamName" },
            { label: "Iskustvo", value: (item) => `${item.experienceYears} godina` }
        ],
        formFields: [
            { name: "firstName", label: "Ime", type: "text", required: true },
            { name: "lastName", label: "Prezime", type: "text", required: true },
            { name: "experienceYears", label: "Godine iskustva", type: "number", required: true, min: 0 },
            { name: "teamId", label: "Tim", type: "select", required: true, optionsSource: "teams" }
        ]
    },
    matches: {
        label: "Utakmice",
        endpoint: "/matches",
        createLabel: "Nova utakmica",
        cardTitle: (item) => `${item.homeTeamName} vs ${item.awayTeamName}`,
        cardMeta: (item) => formatDateTime(item.matchDate),
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Domacin", key: "homeTeamName" },
            { label: "Gost", key: "awayTeamName" },
            { label: "Rezultat", value: (item) => `${item.homeScore} : ${item.awayScore}` }
        ],
        formFields: [
            { name: "homeTeamId", label: "Domaci tim", type: "select", required: true, optionsSource: "teams" },
            { name: "awayTeamId", label: "Gostujuci tim", type: "select", required: true, optionsSource: "teams" },
            { name: "matchDate", label: "Datum i vreme", type: "datetime-local", required: true },
            { name: "homeScore", label: "Poeni domacina", type: "number", required: true, min: 0 },
            { name: "awayScore", label: "Poeni gosta", type: "number", required: true, min: 0 }
        ],
        extraActions: [
            { key: "export", label: "Export" }
        ]
    },
    playerStatistics: {
        label: "Statistike",
        endpoint: "/player-statistics",
        createLabel: "Nova statistika",
        cardTitle: (item) => `${item.playerName} - ${formatShortDate(item.matchDate)}`,
        cardMeta: (item) => `${item.points} poena, ${item.fantasyPoints} fantasy`,
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Igrac", key: "playerName" },
            { label: "Poeni", key: "points" },
            { label: "Asistencije", key: "assists" },
            { label: "Skokovi", key: "rebounds" },
            { label: "Fantasy", key: "fantasyPoints" }
        ],
        formFields: [
            { name: "playerId", label: "Igrac", type: "select", required: true, optionsSource: "players" },
            { name: "matchId", label: "Utakmica", type: "select", required: true, optionsSource: "matches" },
            { name: "points", label: "Poeni", type: "number", required: true, min: 0 },
            { name: "assists", label: "Asistencije", type: "number", required: true, min: 0 },
            { name: "rebounds", label: "Skokovi", type: "number", required: true, min: 0 },
            { name: "minutes", label: "Minuti", type: "number", required: true, min: 0 },
            { name: "steals", label: "Ukradene", type: "number", required: true, min: 0 },
            { name: "blocks", label: "Blokade", type: "number", required: true, min: 0 },
            { name: "turnovers", label: "Turnovers", type: "number", required: true, min: 0 }
        ]
    },
    injuries: {
        label: "Povrede",
        endpoint: "/injuries",
        createLabel: "Nova povreda",
        cardTitle: (item) => item.playerName,
        cardMeta: (item) => item.description,
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Datum", value: (item) => formatShortDate(item.injuryDate) },
            { label: "Oporavak", value: (item) => item.recoveryDate ? formatShortDate(item.recoveryDate) : "Nije unet" },
            { label: "Status", value: (item) => item.isActive ? "Aktivna" : "Zatvorena", badge: true }
        ],
        formFields: [
            { name: "playerId", label: "Igrac", type: "select", required: true, optionsSource: "players" },
            { name: "description", label: "Opis", type: "textarea", required: true },
            { name: "injuryDate", label: "Datum povrede", type: "date", required: true },
            { name: "recoveryDate", label: "Datum oporavka", type: "date" },
            { name: "isActive", label: "Aktivna povreda", type: "checkbox" }
        ]
    },
    fantasyTeams: {
        label: "Fantasy timovi",
        endpoint: "/fantasy-teams",
        createLabel: "Novi fantasy tim",
        cardTitle: (item) => item.name,
        cardMeta: (item) => `${item.coachName} - starteri ${item.starterCount} / klupa ${item.benchCount}`,
        displayFields: [
            { label: "ID", key: "id" },
            { label: "Trener", key: "coachName" },
            { label: "Starteri", key: "starterCount" },
            { label: "Klupa", key: "benchCount" }
        ],
        formFields: [
            { name: "name", label: "Naziv", type: "text", required: true },
            { name: "coachId", label: "Trener", type: "select", required: true, optionsSource: "coaches" }
        ],
        extraActions: [
            { key: "details", label: "Detalji" },
            { key: "points", label: "Poeni" },
            { key: "addPlayer", label: "Dodaj igraca" },
            { key: "export", label: "Export" }
        ]
    }
};

document.addEventListener("DOMContentLoaded", init);
document.addEventListener("click", handleClick);
document.addEventListener("submit", handleSubmit);
document.addEventListener("input", handleInput);

function init() {
    restoreSession();
    renderApp();

    if (isAuthenticated()) {
        void refreshCurrentView(true);
    }
}

function restoreSession() {
    const raw = localStorage.getItem(storageKey);
    if (!raw) {
        return;
    }

    try {
        const session = JSON.parse(raw);
        state.token = session.token ?? "";
        state.userName = session.userName ?? "";
        state.role = session.role ?? "";
    } catch {
        localStorage.removeItem(storageKey);
    }
}

function persistSession() {
    if (!state.token) {
        localStorage.removeItem(storageKey);
        return;
    }

    localStorage.setItem(storageKey, JSON.stringify({
        token: state.token,
        userName: state.userName,
        role: state.role
    }));
}

function isAuthenticated() {
    return Boolean(state.token);
}

function isAdmin() {
    return state.role === "Admin";
}

function renderApp() {
    renderNav();
    renderSessionSummary();
    renderHeroMetrics();
    renderAuthSection();
    renderWorkspace();
}

function renderNav() {
    const nav = document.getElementById("navPanel");
    if (!nav) {
        return;
    }

    const items = [
        { key: "dashboard", label: "Pregled", count: "" },
        ...Object.entries(resources).map(([key, config]) => ({
            key,
            label: config.label,
            count: Array.isArray(state.resources[key]) ? state.resources[key].length : ""
        }))
    ];

    nav.innerHTML = items.map((item) => {
        const activeClass = state.currentView === item.key ? "nav-button nav-button--active" : "nav-button";
        const pill = item.count !== "" ? `<span class="nav-button__pill">${escapeHtml(String(item.count))}</span>` : "";

        return `
            <button class="${activeClass}" type="button" data-view="${escapeHtml(item.key)}">
                <span>${escapeHtml(item.label)}</span>
                ${pill}
            </button>
        `;
    }).join("");
}

function renderSessionSummary() {
    const target = document.getElementById("sessionSummary");
    if (!target) {
        return;
    }

    if (!isAuthenticated()) {
        target.innerHTML = `
            <div class="session-chip">
                <span class="session-chip__status">Nema aktivne sesije</span>
                <p class="session-chip__title">Prijavi se kao administrator</p>
                <p class="session-chip__copy">
                    Kada se prijavis, frontend otkljucava sve preglede, forme i eksport opcije
                    bez dodatnog podesavanja.
                </p>
            </div>
        `;
        return;
    }

    target.innerHTML = `
        <div class="session-chip">
            <span class="session-chip__status session-chip__status--online">Sesija aktivna</span>
            <p class="session-chip__title">${escapeHtml(state.userName || "Prijavljen korisnik")}</p>
            <p class="session-chip__copy">
                Uloga: <strong>${escapeHtml(state.role || "Nepoznata")}</strong><br>
                Mutacije su omogucene samo za admin nalog.
            </p>
            <div class="button-row">
                <button class="button button--ghost" type="button" data-action="logout">Odjava</button>
            </div>
        </div>
    `;
}

function renderHeroMetrics() {
    const target = document.getElementById("heroMetrics");
    if (!target) {
        return;
    }

    const items = isAuthenticated()
        ? [
            { label: "Timovi", value: String((state.resources.teams ?? []).length), hint: "Struktura lige i rosteri." },
            { label: "Igraci", value: String((state.resources.players ?? []).length), hint: "Osnovni roster i pozicije." },
            { label: "Utakmice", value: String((state.resources.matches ?? []).length), hint: "Rezultati i zakazani mecevi." },
            { label: "Fantasy timovi", value: String((state.resources.fantasyTeams ?? []).length), hint: "Ekipa za fantasy obracun." }
        ]
        : [
            { label: "JWT login", value: "1", hint: "Frontend cuva token i salje ga ka zasticenim rutama." },
            { label: "CQRS ekran", value: "7", hint: "Svi glavni resursi imaju svoj pregled." },
            { label: "Export", value: "2", hint: "Excel izvoz za meceve i fantasy timove." },
            { label: "Admin seed", value: "Ready", hint: "Prijava radi bez dodatne pripreme." }
        ];

    target.innerHTML = items.map((item) => `
        <article class="metric-card">
            <p class="section-label">${escapeHtml(item.label)}</p>
            <span class="metric-card__value">${escapeHtml(item.value)}</span>
            <p class="metric-card__hint">${escapeHtml(item.hint)}</p>
        </article>
    `).join("");
}

function renderAuthSection() {
    const target = document.getElementById("authSection");
    if (!target) {
        return;
    }

    if (isAuthenticated()) {
        target.innerHTML = `
            <div class="auth-grid">
                <article class="auth-card">
                    <p class="section-label">Aktivna sesija</p>
                    <h2 class="card-title">Frontend je spreman za rad</h2>
                    <p class="card-subtitle">
                        Kroz navigaciju sa leve strane mozes brzo da predjes na timove, igrace,
                        statistike, povrede i fantasy module.
                    </p>
                    <div class="auth-note">
                        Ako zelis samo pregled bez menjanja podataka, mozes koristiti i obican korisnicki nalog
                        preko registracije. Za kreiranje, izmenu i brisanje koristi admin nalog.
                    </div>
                </article>

                <article class="auth-card">
                    <p class="section-label">Sesija</p>
                    <h2 class="card-title">${escapeHtml(state.userName)}</h2>
                    <p class="card-subtitle">
                        Trenutna uloga: <strong>${escapeHtml(state.role || "Nepoznata")}</strong>
                    </p>
                    <div class="button-row">
                        <button class="button button--ghost" type="button" data-action="logout">Odjava</button>
                        <button class="button button--soft" type="button" data-view="dashboard">Nazad na pregled</button>
                    </div>
                </article>
            </div>
        `;
        return;
    }

    target.innerHTML = `
        <div class="auth-grid">
            <article class="auth-card">
                <p class="section-label">Prijava</p>
                <h2 class="card-title">Uloguj se da otkljucas rad sa API-jem</h2>
                <p class="card-subtitle">
                    Frontend koristi iste auth rute kao backend i cuva JWT u lokalnoj sesiji browsera.
                </p>

                <form class="form-grid" data-form="login">
                    <div class="field">
                        <label for="loginUserName">Korisnicko ime</label>
                        <input id="loginUserName" name="userName" type="text" placeholder="admin" required>
                    </div>
                    <div class="field">
                        <label for="loginPassword">Lozinka</label>
                        <input id="loginPassword" name="password" type="password" placeholder="Admin123!" required>
                    </div>

                    <div class="field field--full">
                        <div class="button-row">
                            <button class="button" type="submit">Prijava</button>
                            <button class="button button--ghost" type="button" data-action="fill-admin">Popuni admin podatke</button>
                        </div>
                    </div>
                </form>
            </article>

            <article class="auth-card">
                <p class="section-label">Registracija</p>
                <h2 class="card-title">Po potrebi napravi i obicnog korisnika</h2>
                <p class="card-subtitle">
                    Korisnicka registracija je zgodna za demonstraciju Identity dela projekta.
                </p>

                <details>
                    <summary>Otvori registracionu formu</summary>
                    <div class="details-body">
                        <form class="form-grid" data-form="register">
                            <div class="field">
                                <label for="registerUserName">Korisnicko ime</label>
                                <input id="registerUserName" name="userName" type="text" required>
                            </div>
                            <div class="field">
                                <label for="registerEmail">Email</label>
                                <input id="registerEmail" name="email" type="email" required>
                            </div>
                            <div class="field">
                                <label for="registerFirstName">Ime</label>
                                <input id="registerFirstName" name="firstName" type="text" required>
                            </div>
                            <div class="field">
                                <label for="registerLastName">Prezime</label>
                                <input id="registerLastName" name="lastName" type="text" required>
                            </div>
                            <div class="field field--full">
                                <label for="registerPassword">Lozinka</label>
                                <input id="registerPassword" name="password" type="password" minlength="6" required>
                            </div>
                            <div class="field field--full">
                                <button class="button" type="submit">Registruj korisnika</button>
                            </div>
                        </form>
                    </div>
                </details>
            </article>
        </div>
    `;
}

function renderWorkspace() {
    const target = document.getElementById("workspace");
    if (!target) {
        return;
    }

    if (!isAuthenticated()) {
        target.innerHTML = `
            <article class="empty-state">
                <p class="section-label">Zakljucan radni prostor</p>
                <h2 class="empty-state__title">Prvo se prijavi da bi frontend povukao podatke iz API-ja</h2>
                <p class="empty-state__copy">
                    Posle prijave automatski ces dobiti dashboard sa osnovnim brojkama,
                    top scorer izvestajem i pregledom aktivnih povreda.
                </p>
            </article>
        `;
        return;
    }

    if (state.loading) {
        target.innerHTML = `
            <article class="panel">
                <p class="section-label">Ucitavanje</p>
                <h2 class="card-title">Pripremam podatke za prikaz...</h2>
            </article>
        `;
        return;
    }

    if (state.currentView === "dashboard") {
        target.innerHTML = renderDashboard();
        return;
    }

    const config = resources[state.currentView];
    const records = Array.isArray(state.resources[state.currentView]) ? state.resources[state.currentView] : [];
    const filtered = filterRecords(records, config, state.search);

    target.innerHTML = `
        <div class="workspace-grid">
            <article class="panel">
                <div class="toolbar">
                    <div class="toolbar__copy">
                        <p class="section-label">${escapeHtml(config.label)}</p>
                        <h2 class="card-title">${escapeHtml(config.label)} u jednom mestu</h2>
                        <p class="card-subtitle">
                            Prikaz, pretraga i osnovne akcije nad resursom <code>/${escapeHtml(config.endpoint.replace("/", ""))}</code>.
                        </p>
                    </div>

                    <div class="toolbar__actions">
                        <div class="toolbar__search">
                            <input
                                type="search"
                                value="${escapeHtml(state.search)}"
                                data-input="resource-search"
                                placeholder="Pretrazi prikazane kartice">
                        </div>
                        <button class="button button--ghost" type="button" data-action="refresh-view">Osvezi</button>
                        ${isAdmin() ? `<button class="button" type="button" data-action="new-record">${escapeHtml(config.createLabel)}</button>` : ""}
                    </div>
                </div>
            </article>

            ${state.formState && state.formState.resourceKey === state.currentView ? renderPrimaryForm(config) : ""}
            ${state.detailState ? renderDetailPanel() : ""}

            ${filtered.length
                ? `<div class="record-list">${filtered.map((record) => renderRecordCard(config, record)).join("")}</div>`
                : `
                    <article class="empty-state">
                        <h3 class="empty-state__title">Nema rezultata za prikaz</h3>
                        <p class="empty-state__copy">
                            ${state.search
                                ? "Promeni pojam za pretragu ili osvezi podatke."
                                : "Jos nema unetih podataka ili ih ovaj nalog trenutno ne vidi."}
                        </p>
                    </article>
                `}
        </div>
    `;
}

function renderDashboard() {
    const topScorers = state.dashboard?.topScorers ?? [];
    const activeInjuries = state.dashboard?.activeInjuries ?? [];

    return `
        <div class="workspace-grid">
            <article class="panel">
                <div class="toolbar">
                    <div class="toolbar__copy">
                        <p class="section-label">Pregled</p>
                        <h2 class="card-title">Brzi dashboard za svakodnevni rad</h2>
                        <p class="card-subtitle">
                            Sa jedne strane su najbitnije brojke, a sa druge dve sekcije koje odmah
                            vode ka akcijama u sistemu.
                        </p>
                    </div>

                    <div class="toolbar__actions">
                        <button class="button button--ghost" type="button" data-action="refresh-view">Osvezi pregled</button>
                    </div>
                </div>
            </article>

            <div class="dashboard-columns">
                <article class="dashboard-section">
                    <p class="section-label">Top scoreri</p>
                    <h3 class="card-title">Najefikasniji igraci</h3>
                    <div class="dashboard-section__list">
                        ${topScorers.length
                            ? topScorers.map((item) => `
                                <div class="mini-card">
                                    <h4 class="mini-card__title">${escapeHtml(item.playerName)}</h4>
                                    <p class="mini-card__copy">
                                        ${escapeHtml(item.teamName)}<br>
                                        Ukupno poena: <strong>${escapeHtml(String(item.totalPoints))}</strong><br>
                                        Odigrano utakmica: ${escapeHtml(String(item.gamesPlayed))}
                                    </p>
                                </div>
                            `).join("")
                            : `<p class="muted">Nema dovoljno podataka za top scorere.</p>`}
                    </div>
                </article>

                <article class="dashboard-section">
                    <p class="section-label">Aktivne povrede</p>
                    <h3 class="card-title">Ko je trenutno van stroja</h3>
                    <div class="dashboard-section__list">
                        ${activeInjuries.length
                            ? activeInjuries.map((item) => `
                                <div class="mini-card">
                                    <h4 class="mini-card__title">${escapeHtml(item.playerName)}</h4>
                                    <p class="mini-card__copy">
                                        ${escapeHtml(item.description)}<br>
                                        Povreda: ${escapeHtml(formatShortDate(item.injuryDate))}
                                    </p>
                                </div>
                            `).join("")
                            : `<p class="muted">Nema aktivnih povreda.</p>`}
                    </div>
                </article>
            </div>

            <article class="panel">
                <p class="section-label">Preporuceni koraci</p>
                <div class="button-row">
                    <button class="button" type="button" data-view="teams">Otvori timove</button>
                    <button class="button button--soft" type="button" data-view="players">Pregled igraca</button>
                    <button class="button button--soft" type="button" data-view="matches">Pregled utakmica</button>
                    <button class="button button--soft" type="button" data-view="fantasyTeams">Fantasy timovi</button>
                </div>
            </article>
        </div>
    `;
}

function renderRecordCard(config, record) {
    const actions = [];

    for (const action of config.extraActions ?? []) {
        actions.push(`
            <button
                class="button button--soft"
                type="button"
                data-row-action="${escapeHtml(action.key)}"
                data-id="${escapeHtml(String(record.id))}">
                ${escapeHtml(action.label)}
            </button>
        `);
    }

    if (isAdmin()) {
        actions.push(`
            <button class="button button--ghost" type="button" data-row-action="edit" data-id="${escapeHtml(String(record.id))}">
                Izmeni
            </button>
        `);
        actions.push(`
            <button class="button button--danger" type="button" data-row-action="delete" data-id="${escapeHtml(String(record.id))}">
                Obrisi
            </button>
        `);
    }

    const stats = config.displayFields.map((field) => renderStatField(field, record)).join("");

    return `
        <article class="record-card">
            <div class="record-card__head">
                <div>
                    <h3 class="record-card__title">${escapeHtml(config.cardTitle(record))}</h3>
                    <p class="record-card__meta">${escapeHtml(config.cardMeta(record))}</p>
                </div>
                <span class="badge">#${escapeHtml(String(record.id))}</span>
            </div>

            <div class="record-card__grid">
                ${stats}
            </div>

            ${actions.length ? `<div class="record-card__actions">${actions.join("")}</div>` : ""}
        </article>
    `;
}

function renderStatField(field, record) {
    const rawValue = field.value ? field.value(record) : record[field.key];
    const formatted = rawValue === null || rawValue === undefined || rawValue === "" ? "Nije uneto" : String(rawValue);
    const badgeClass = field.badge
        ? formatted.toLowerCase().includes("aktiv") || formatted.toLowerCase().includes("starter")
            ? "badge badge--success"
            : "badge badge--danger"
        : "";

    return `
        <div class="stat">
            <span class="stat__label">${escapeHtml(field.label)}</span>
            ${field.badge
                ? `<span class="${badgeClass}">${escapeHtml(formatted)}</span>`
                : `<span class="stat__value">${escapeHtml(formatted)}</span>`}
        </div>
    `;
}

function renderPrimaryForm(config) {
    const record = state.formState?.record ?? null;
    const mode = record ? "Izmena" : "Kreiranje";

    return `
        <article class="panel">
            <div class="toolbar">
                <div class="toolbar__copy">
                    <p class="section-label">${escapeHtml(mode)}</p>
                    <h3 class="card-title">${escapeHtml(record ? `Izmeni ${config.label.toLowerCase()}` : config.createLabel)}</h3>
                </div>
                <div class="toolbar__actions">
                    <button class="button button--ghost" type="button" data-action="close-form">Zatvori</button>
                </div>
            </div>

            <form class="form-grid" data-form="resource">
                ${config.formFields.map((field) => renderFormField(field, record)).join("")}
                <div class="field field--full">
                    <div class="button-row">
                        <button class="button" type="submit">${record ? "Sacuvaj izmene" : "Kreiraj zapis"}</button>
                        <button class="button button--ghost" type="button" data-action="close-form">Odustani</button>
                    </div>
                </div>
            </form>
        </article>
    `;
}

function renderFormField(field, record) {
    const rawValue = record ? record[field.name] : "";
    const value = getFieldValueForInput(field, rawValue);
    const required = field.required ? "required" : "";
    const fieldClass = field.type === "textarea" ? "field field--full" : "field";

    if (field.type === "checkbox") {
        return `
            <div class="${fieldClass}">
                <label>${escapeHtml(field.label)}</label>
                <label class="checkbox">
                    <input type="checkbox" name="${escapeHtml(field.name)}" ${value ? "checked" : ""}>
                    <span>${escapeHtml(field.label)}</span>
                </label>
            </div>
        `;
    }

    if (field.type === "select") {
        const options = state.options[field.optionsSource] ?? [];
        const normalizedValue = value === null || value === undefined ? "" : String(value);

        return `
            <div class="${fieldClass}">
                <label for="field-${escapeHtml(field.name)}">${escapeHtml(field.label)}</label>
                <select id="field-${escapeHtml(field.name)}" name="${escapeHtml(field.name)}" ${required}>
                    <option value="">Izaberi...</option>
                    ${options.map((option) => `
                        <option value="${escapeHtml(String(option.value))}" ${String(option.value) === normalizedValue ? "selected" : ""}>
                            ${escapeHtml(option.label)}
                        </option>
                    `).join("")}
                </select>
            </div>
        `;
    }

    if (field.type === "textarea") {
        return `
            <div class="${fieldClass}">
                <label for="field-${escapeHtml(field.name)}">${escapeHtml(field.label)}</label>
                <textarea id="field-${escapeHtml(field.name)}" name="${escapeHtml(field.name)}" ${required}>${escapeHtml(value ?? "")}</textarea>
            </div>
        `;
    }

    const min = field.min !== undefined ? `min="${escapeHtml(String(field.min))}"` : "";
    const step = field.step ? `step="${escapeHtml(field.step)}"` : "";
    const placeholder = field.placeholder ? `placeholder="${escapeHtml(field.placeholder)}"` : "";

    return `
        <div class="${fieldClass}">
            <label for="field-${escapeHtml(field.name)}">${escapeHtml(field.label)}</label>
            <input
                id="field-${escapeHtml(field.name)}"
                name="${escapeHtml(field.name)}"
                type="${escapeHtml(field.type)}"
                value="${escapeHtml(value ?? "")}"
                ${placeholder}
                ${required}
                ${min}
                ${step}>
        </div>
    `;
}

function renderDetailPanel() {
    return `
        <article class="detail-card">
            <div class="detail-card__header">
                <div>
                    <p class="section-label">Detalji</p>
                    <h3 class="card-title">${escapeHtml(state.detailState.title)}</h3>
                </div>
                <button class="button button--ghost" type="button" data-action="close-detail">Zatvori</button>
            </div>
            <div class="detail-card__body">${state.detailState.html}</div>
        </article>
    `;
}

function handleInput(event) {
    if (event.target.matches('[data-input="resource-search"]')) {
        state.search = event.target.value.trim();
        renderWorkspace();
    }
}

function handleClick(event) {
    const viewButton = event.target.closest("[data-view]");
    if (viewButton) {
        const view = viewButton.dataset.view;
        void setView(view);
        return;
    }

    const actionButton = event.target.closest("[data-action]");
    if (actionButton) {
        const action = actionButton.dataset.action;

        if (action === "fill-admin") {
            fillAdminCredentials();
            return;
        }

        if (action === "logout") {
            logout();
            return;
        }

        if (action === "new-record") {
            void openPrimaryForm(state.currentView, null);
            return;
        }

        if (action === "refresh-view") {
            void refreshCurrentView(true);
            return;
        }

        if (action === "close-form") {
            state.formState = null;
            renderWorkspace();
            return;
        }

        if (action === "close-detail") {
            state.detailState = null;
            renderWorkspace();
        }
    }

    const rowAction = event.target.closest("[data-row-action]");
    if (rowAction) {
        const action = rowAction.dataset.rowAction;
        const id = Number(rowAction.dataset.id);
        void handleRowAction(action, id);
    }
}

function handleSubmit(event) {
    const form = event.target;
    if (!(form instanceof HTMLFormElement)) {
        return;
    }

    event.preventDefault();

    if (form.dataset.form === "login") {
        void login(form);
        return;
    }

    if (form.dataset.form === "register") {
        void register(form);
        return;
    }

    if (form.dataset.form === "resource") {
        void saveResource(form);
        return;
    }

    if (form.dataset.form === "fantasy-add-player") {
        void addFantasyPlayer(form);
    }
}

async function setView(view) {
    state.currentView = view;
    state.search = "";
    state.formState = null;
    state.detailState = null;
    renderApp();

    if (isAuthenticated()) {
        await refreshCurrentView(false);
    }
}

async function refreshCurrentView(force) {
    if (!isAuthenticated()) {
        return;
    }

    state.loading = true;
    renderWorkspace();

    try {
        await loadSupportingResources(force);

        if (state.currentView === "dashboard") {
            await loadDashboard(force);
        } else {
            await loadResource(state.currentView, force);
        }
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    } finally {
        state.loading = false;
        renderApp();
    }
}

async function loadSupportingResources(force) {
    const supportKeys = ["teams", "players", "coaches", "matches", "fantasyTeams"];
    const tasks = supportKeys.map((key) => loadResource(key, force));
    await Promise.all(tasks);
}

async function loadDashboard(force) {
    if (state.dashboard && !force) {
        return;
    }

    const [topScorers, activeInjuries] = await Promise.all([
        apiRequest("/player-statistics/top-scorers?count=5"),
        apiRequest("/injuries/active")
    ]);

    state.dashboard = {
        topScorers,
        activeInjuries
    };
}

async function loadResource(key, force) {
    if (state.resources[key] && !force) {
        return;
    }

    const config = resources[key];
    if (!config) {
        return;
    }

    const data = await apiRequest(config.endpoint);
    state.resources[key] = Array.isArray(data) ? data : [];
}

async function login(form) {
    const payload = Object.fromEntries(new FormData(form).entries());

    try {
        const response = await apiRequest("/auth/login", {
            method: "POST",
            body: payload,
            auth: false
        });

        state.token = response.token;
        state.userName = response.userName;
        state.role = response.role;
        persistSession();

        state.currentView = "dashboard";
        state.resources = {};
        state.dashboard = null;
        state.formState = null;
        state.detailState = null;

        form.reset();
        showToast("Prijava je uspesna.", "success");

        renderApp();
        await refreshCurrentView(true);
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    }
}

async function register(form) {
    const payload = Object.fromEntries(new FormData(form).entries());

    try {
        const response = await apiRequest("/auth/register", {
            method: "POST",
            body: payload,
            auth: false
        });

        state.token = response.token;
        state.userName = response.userName;
        state.role = response.role;
        persistSession();

        state.currentView = "dashboard";
        state.resources = {};
        state.dashboard = null;
        state.formState = null;
        state.detailState = null;

        form.reset();
        showToast("Korisnik je registrovan i prijavljen.", "success");

        renderApp();
        await refreshCurrentView(true);
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    }
}

function logout() {
    state.token = "";
    state.userName = "";
    state.role = "";
    state.resources = {};
    state.options = {};
    state.dashboard = null;
    state.formState = null;
    state.detailState = null;
    state.search = "";

    persistSession();
    renderApp();
    showToast("Odjava je uspesna.", "info");
}

async function openPrimaryForm(resourceKey, record) {
    const config = resources[resourceKey];
    if (!config) {
        return;
    }

    await ensureOptions(config.formFields);
    state.formState = { resourceKey, record };
    state.detailState = null;
    renderWorkspace();
}

async function ensureOptions(fields) {
    const tasks = fields
        .filter((field) => field.type === "select" && field.optionsSource)
        .map((field) => getOptions(field.optionsSource));

    await Promise.all(tasks);
}

async function getOptions(source) {
    if (state.options[source]) {
        return state.options[source];
    }

    await loadResource(source, false);
    const records = state.resources[source] ?? [];
    state.options[source] = records.map((item) => ({
        value: item.id,
        label: getOptionLabel(source, item)
    }));

    return state.options[source];
}

function getOptionLabel(source, item) {
    switch (source) {
        case "teams":
            return `${item.name} (${item.city})`;
        case "players":
            return `${item.firstName} ${item.lastName} - ${item.teamName}`;
        case "coaches":
            return `${item.firstName} ${item.lastName} - ${item.teamName}`;
        case "matches":
            return `${item.homeTeamName} vs ${item.awayTeamName} (${formatShortDate(item.matchDate)})`;
        case "fantasyTeams":
            return item.name;
        default:
            return item.name ?? item.id;
    }
}

async function saveResource(form) {
    if (!state.formState) {
        return;
    }

    const config = resources[state.formState.resourceKey];
    const payload = collectFormData(form, config.formFields);
    const isEdit = Boolean(state.formState.record);
    const id = state.formState.record?.id;

    try {
        const response = await apiRequest(
            isEdit ? `${config.endpoint}/${id}` : config.endpoint,
            {
                method: isEdit ? "PUT" : "POST",
                body: payload
            });

        await loadResource(state.formState.resourceKey, true);
        state.formState = null;
        state.detailState = response ? {
            title: isEdit ? "Sacuvane izmene" : "Novi zapis",
            html: renderKeyValueGrid(response)
        } : null;

        state.options = {};
        await loadSupportingResources(true);

        if (state.currentView === "dashboard") {
            state.dashboard = null;
            await loadDashboard(true);
        }

        showToast(isEdit ? "Zapis je uspesno izmenjen." : "Zapis je uspesno kreiran.", "success");
        renderApp();
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    }
}

function collectFormData(form, fields) {
    const formData = new FormData(form);
    const payload = {};

    for (const field of fields) {
        if (field.type === "checkbox") {
            payload[field.name] = form.elements[field.name].checked;
            continue;
        }

        const rawValue = formData.get(field.name);
        if (rawValue === null) {
            payload[field.name] = null;
            continue;
        }

        if (field.type === "number") {
            payload[field.name] = rawValue === "" ? null : Number(rawValue);
            continue;
        }

        if (field.type === "select") {
            payload[field.name] = rawValue === "" ? null : Number(rawValue);
            continue;
        }

        if (field.type === "datetime-local") {
            payload[field.name] = rawValue ? new Date(String(rawValue)).toISOString() : null;
            continue;
        }

        payload[field.name] = String(rawValue);
    }

    return payload;
}

async function handleRowAction(action, id) {
    const resourceKey = state.currentView;
    const config = resources[resourceKey];
    const record = (state.resources[resourceKey] ?? []).find((item) => item.id === id);

    if (!config || !record) {
        return;
    }

    if (action === "edit") {
        await openPrimaryForm(resourceKey, record);
        return;
    }

    if (action === "delete") {
        await deleteRecord(config, id);
        return;
    }

    try {
        switch (`${resourceKey}:${action}`) {
            case "teams:players": {
                const players = await apiRequest(`/teams/${id}/players`);
                state.detailState = {
                    title: `Igraci tima ${record.name}`,
                    html: renderCollectionCards(players, (player) => ({
                        title: `${player.firstName} ${player.lastName}`,
                        copy: `${player.position} - broj ${player.jerseyNumber} - ${player.height} cm`
                    }))
                };
                break;
            }

            case "players:statistics": {
                const statistics = await apiRequest(`/players/${id}/statistics`);
                state.detailState = {
                    title: `Statistike za ${record.firstName} ${record.lastName}`,
                    html: renderCollectionCards(statistics, (item) => ({
                        title: `Utakmica #${item.matchId}`,
                        copy: `${formatShortDate(item.matchDate)} - ${item.points} poena, ${item.assists} ast, ${item.rebounds} sk, fantasy ${item.fantasyPoints}`
                    }))
                };
                break;
            }

            case "players:totals": {
                const totals = await apiRequest(`/players/${id}/totals`);
                state.detailState = {
                    title: `Ukupna statistika za ${totals.playerName}`,
                    html: renderKeyValueGrid({
                        "Odigrane utakmice": totals.gamesPlayed,
                        "Poeni": totals.points,
                        "Asistencije": totals.assists,
                        "Skokovi": totals.rebounds,
                        "Minuti": totals.minutes,
                        "Ukradene": totals.steals,
                        "Blokade": totals.blocks,
                        "Turnovers": totals.turnovers,
                        "Fantasy poeni": totals.fantasyPoints
                    })
                };
                break;
            }

            case "matches:export": {
                await downloadFile(`/matches/${id}/statistics/export`, `match-statistics-${id}.xlsx`);
                return;
            }

            case "fantasyTeams:details": {
                const details = await apiRequest(`/fantasy-teams/${id}`);
                state.detailState = {
                    title: `Detalji fantasy tima ${details.name}`,
                    html: renderFantasyDetails(details)
                };
                break;
            }

            case "fantasyTeams:points": {
                const points = await apiRequest(`/fantasy-teams/${id}/points`);
                state.detailState = {
                    title: `Poeni za ${points.fantasyTeamName}`,
                    html: renderKeyValueGrid({
                        "Fantasy tim": points.fantasyTeamName,
                        "Ukupni fantasy poeni": points.totalFantasyPoints
                    })
                };
                break;
            }

            case "fantasyTeams:addPlayer": {
                await getOptions("players");
                state.detailState = {
                    title: `Dodavanje igraca u ${record.name}`,
                    html: renderFantasyAddPlayerForm(id)
                };
                break;
            }

            case "fantasyTeams:export": {
                await downloadFile(`/fantasy-teams/${id}/export`, `fantasy-team-${id}.xlsx`);
                return;
            }

            default:
                return;
        }

        renderWorkspace();
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    }
}

async function deleteRecord(config, id) {
    const confirmed = window.confirm("Da li sigurno zelis da obrises ovaj zapis?");
    if (!confirmed) {
        return;
    }

    try {
        await apiRequest(`${config.endpoint}/${id}`, {
            method: "DELETE"
        });

        await loadResource(state.currentView, true);
        state.detailState = null;
        state.options = {};
        await loadSupportingResources(true);

        state.dashboard = null;
        await loadDashboard(true);

        showToast("Zapis je uspesno obrisan.", "success");
        renderApp();
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    }
}

async function addFantasyPlayer(form) {
    const teamId = Number(form.dataset.teamId);
    const payload = {
        playerId: Number(form.elements.playerId.value),
        role: Number(form.elements.role.value)
    };

    try {
        const details = await apiRequest(`/fantasy-teams/${teamId}/players`, {
            method: "POST",
            body: payload
        });

        await loadResource("fantasyTeams", true);
        await loadSupportingResources(true);
        state.detailState = {
            title: `Azurirani fantasy tim ${details.name}`,
            html: renderFantasyDetails(details)
        };

        state.dashboard = null;
        await loadDashboard(true);

        showToast("Igrac je dodat u fantasy tim.", "success");
        renderApp();
    } catch (error) {
        showToast(readErrorMessage(error), "error");
    }
}

function renderFantasyDetails(details) {
    const players = details.players ?? [];

    return `
        <div class="detail-grid">
            <div class="stat">
                <span class="stat__label">Naziv tima</span>
                <span class="stat__value">${escapeHtml(details.name)}</span>
            </div>
            <div class="stat">
                <span class="stat__label">Trener</span>
                <span class="stat__value">${escapeHtml(details.coachName)}</span>
            </div>
        </div>
        <hr class="divider">
        <div class="dashboard-section__list">
            ${players.length
                ? players.map((player) => `
                    <div class="mini-card">
                        <h4 class="mini-card__title">${escapeHtml(player.playerName)}</h4>
                        <p class="mini-card__copy">
                            ${escapeHtml(player.teamName)}<br>
                            Uloga: <strong>${escapeHtml(player.role)}</strong><br>
                            Ukupno fantasy poena: ${escapeHtml(String(player.totalFantasyPoints))}
                        </p>
                    </div>
                `).join("")
                : `<p class="muted">Fantasy tim jos nema dodatih igraca.</p>`}
        </div>
    `;
}

function renderFantasyAddPlayerForm(teamId) {
    const options = state.options.players ?? [];

    return `
        <form class="form-grid" data-form="fantasy-add-player" data-team-id="${escapeHtml(String(teamId))}">
            <div class="field">
                <label for="fantasyPlayerId">Igrac</label>
                <select id="fantasyPlayerId" name="playerId" required>
                    <option value="">Izaberi igraca...</option>
                    ${options.map((option) => `
                        <option value="${escapeHtml(String(option.value))}">${escapeHtml(option.label)}</option>
                    `).join("")}
                </select>
            </div>

            <div class="field">
                <label for="fantasyRole">Uloga</label>
                <select id="fantasyRole" name="role" required>
                    <option value="1">Starter</option>
                    <option value="2">Bench</option>
                </select>
            </div>

            <div class="field field--full">
                <div class="button-row">
                    <button class="button" type="submit">Dodaj igraca</button>
                </div>
            </div>
        </form>
    `;
}

function renderCollectionCards(items, transform) {
    if (!items.length) {
        return `<p class="muted">Nema podataka za prikaz.</p>`;
    }

    return `
        <div class="dashboard-section__list">
            ${items.map((item) => {
                const view = transform(item);
                return `
                    <div class="mini-card">
                        <h4 class="mini-card__title">${escapeHtml(view.title)}</h4>
                        <p class="mini-card__copy">${escapeHtml(view.copy)}</p>
                    </div>
                `;
            }).join("")}
        </div>
    `;
}

function renderKeyValueGrid(data) {
    const entries = Array.isArray(data)
        ? data
        : typeof data === "object" && data !== null
            ? Object.entries(data)
            : [];

    return `
        <div class="detail-grid">
            ${entries.map(([key, value]) => `
                <div class="stat">
                    <span class="stat__label">${escapeHtml(String(key))}</span>
                    <span class="stat__value">${escapeHtml(formatMaybeDate(value))}</span>
                </div>
            `).join("")}
        </div>
    `;
}

function filterRecords(records, config, term) {
    if (!term) {
        return records;
    }

    const lowered = term.toLowerCase();

    return records.filter((record) => {
        const haystack = [
            config.cardTitle(record),
            config.cardMeta(record),
            ...config.displayFields.map((field) => field.value ? field.value(record) : record[field.key])
        ]
            .filter((value) => value !== null && value !== undefined)
            .join(" ")
            .toLowerCase();

        return haystack.includes(lowered);
    });
}

function fillAdminCredentials() {
    const userName = document.getElementById("loginUserName");
    const password = document.getElementById("loginPassword");

    if (userName instanceof HTMLInputElement) {
        userName.value = "admin";
    }

    if (password instanceof HTMLInputElement) {
        password.value = "Admin123!";
    }

    showToast("Admin podaci su ubaceni u formu.", "info");
}

async function downloadFile(url, fallbackName) {
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${state.token}`
        }
    });

    if (!response.ok) {
        throw new Error(await parseError(response));
    }

    const blob = await response.blob();
    const objectUrl = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = objectUrl;
    anchor.download = fallbackName;
    anchor.click();
    URL.revokeObjectURL(objectUrl);

    showToast(`Preuzimanje je spremno: ${fallbackName}`, "success");
}

async function apiRequest(url, options = {}) {
    const requestOptions = {
        method: options.method ?? "GET",
        headers: {
            Accept: "application/json",
            ...(options.body ? { "Content-Type": "application/json" } : {}),
            ...(options.auth === false || !state.token ? {} : { Authorization: `Bearer ${state.token}` })
        },
        body: options.body ? JSON.stringify(options.body) : undefined
    };

    const response = await fetch(url, requestOptions);

    if (response.status === 401) {
        logout();
        throw new Error("Sesija je istekla. Prijavi se ponovo.");
    }

    if (response.status === 204) {
        return null;
    }

    if (!response.ok) {
        throw new Error(await parseError(response));
    }

    return response.json();
}

async function parseError(response) {
    const contentType = response.headers.get("content-type") ?? "";
    if (!contentType.includes("application/json")) {
        const text = await response.text();
        return text || "Doslo je do greske pri radu sa API-jem.";
    }

    const payload = await response.json();

    if (typeof payload === "string") {
        return payload;
    }

    if (payload.message) {
        return payload.message;
    }

    if (Array.isArray(payload)) {
        return payload.map((item) => item.errorMessage ?? JSON.stringify(item)).join(" | ");
    }

    return "Operacija nije uspela.";
}

function readErrorMessage(error) {
    if (error instanceof Error) {
        return error.message;
    }

    return "Doslo je do neocekivane greske.";
}

function showToast(message, type) {
    const id = Date.now() + Math.random();
    state.toasts = [...state.toasts, { id, message, type }];
    renderToasts();

    window.setTimeout(() => {
        state.toasts = state.toasts.filter((toast) => toast.id !== id);
        renderToasts();
    }, 3600);
}

function renderToasts() {
    const target = document.getElementById("toastStack");
    if (!target) {
        return;
    }

    target.innerHTML = state.toasts.map((toast) => `
        <div class="toast toast--${escapeHtml(toast.type)}">${escapeHtml(toast.message)}</div>
    `).join("");
}

function getFieldValueForInput(field, rawValue) {
    if (rawValue === null || rawValue === undefined) {
        return field.type === "checkbox" ? false : "";
    }

    if (field.type === "date") {
        return typeof rawValue === "string" ? rawValue.slice(0, 10) : "";
    }

    if (field.type === "datetime-local") {
        return toDateTimeLocal(rawValue);
    }

    if (field.type === "checkbox") {
        return Boolean(rawValue);
    }

    return rawValue;
}

function toDateTimeLocal(value) {
    if (!value) {
        return "";
    }

    const date = new Date(value);
    const offset = date.getTimezoneOffset() * 60000;
    return new Date(date.getTime() - offset).toISOString().slice(0, 16);
}

function formatDateTime(value) {
    if (!value) {
        return "Nije uneto";
    }

    return new Intl.DateTimeFormat("sr-RS", {
        dateStyle: "medium",
        timeStyle: "short"
    }).format(new Date(value));
}

function formatShortDate(value) {
    if (!value) {
        return "Nije uneto";
    }

    return new Intl.DateTimeFormat("sr-RS", {
        dateStyle: "medium"
    }).format(new Date(value));
}

function formatMaybeDate(value) {
    if (Array.isArray(value)) {
        return `${value.length} stavki`;
    }

    if (typeof value === "boolean") {
        return value ? "Da" : "Ne";
    }

    if (typeof value === "string" && /\d{4}-\d{2}-\d{2}T/.test(value)) {
        return formatDateTime(value);
    }

    if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
        return formatShortDate(value);
    }

    if (typeof value === "object" && value !== null) {
        return "Slozen podatak";
    }

    return String(value);
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#39;");
}
