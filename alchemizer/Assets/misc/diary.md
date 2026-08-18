# ALCHEMIZER — Diario di Sviluppo

> Aggiornato al termine del **Giorno 32**.

---

## Visione del Gioco

Alchemizer è un **platformer 2D con elementi RPG** e progressione basata sull'alchimia.

Il fulcro del gioco è la raccolta di **essenze elementali**, il potenziamento del **Catalizzatore** e lo sblocco graduale di abilità e statistiche tramite uno **Skill Tree**.

### Obiettivo della Demo
La demo rappresenta una versione quasi completa delle meccaniche principali, non una piccola porzione del gioco finale.

- ~8–10 ore di gioco
- Poche zone ma ben rifinite
- Maggior parte dei sistemi già presenti e funzionanti
- Il limite principale è il numero di aree e contenuti, non la profondità dei sistemi

---

## Decisioni di Design

### Essenze Elementali
Cinque tipologie: `Air` `Fire` `Water` `Light` `Dark`
- Ottenute dai nemici (drop alla morte)
- Raccoglibili dal giocatore
- Conteggiate tramite Essence Manager centralizzato
- Visibili in UI

### Skill Tree
- Unico albero principale (no 5 alberi separati per elemento)
- Le abilità possono richiedere più tipi di essenza
- Progressione centrata sul Catalizzatore
- Il potenziamento del Catalizzatore sblocca nuovi livelli dell'albero

### Catalizzatore
Nodo radice della progressione. Attuale nodo: `Catalyst Base`.
Roadmap: evolversi in stadi, sbloccare nuove sezioni dell'albero, rappresentare i milestone principali.

### Decisione Risolta (Giorno 6)
Le abilità di movimento si sbloccano tramite **Skill Tree** (ramo Movimento: Air Dash, Gale Step, Wind Flow, Swift Step). Le skill di progressione metroidvania (dash, double jump, ecc.) si sbloccano tramite esplorazione e vengono potenziate dalle skill nel tree.

### Decisione Risolta (Giorno 21)
**L'inventario serve davvero?** Dubbio aperto dal Giorno 16, ora chiuso: l'inventario resta nel gioco ma con uno scope ridotto — è dedicato esclusivamente a chiavi e oggetti chiave. Niente drop generici o consumabili passano da lì (le pozioni, ad esempio, hanno un proprio sistema dedicato a partire dal Giorno 22).

---

## Architettura Tecnica

- Motore: **Unity**, linguaggio **C#**
- Le skill usano **ScriptableObject** (`SkillSO`) — struttura estendibile
- Anche il **Quest System** è basato su ScriptableObject
- Attacco ranged basato su **raycast**, visualizzato con **LineRenderer** (da migliorare esteticamente)
- Essenze gestite da un **Essence Manager** centralizzato
- **SaveManager**: singleton con `DontDestroyOnLoad`, ristrutturato al Giorno 21 per accogliere i nuovi sistemi
- **Core Instability** (pressure): pattern a eventi `OnAttack()` / `OnHit()`, integrato con post-processing URP (vignette, color adjustment, saturazione)
- **Status Effect System**: `statusManager` che ticka gli effetti attivi ogni frame, con sottoclassi dedicate per singolo effetto (es. `poison`), riorganizzato al Giorno 26
- Feedback di combattimento: `HitStopManager` (via `Time.timeScale`), hit-flash nemici (`SpriteRenderer.color`), knockback

---

## Diario

### Giorno 1
**Focus:** fondamenta e movimento base

- Setup progetto Unity
- Controller base del player
- Gestione del terreno
- Movimento destra/sinistra, salto, dash

**Risultato:** il personaggio è controllabile e dispone del movimento principale.

---

### Giorno 2
**Focus:** nemici e primo ciclo di combattimento

- Enemy Base con sistema HP
- Morte del nemico e distruzione GameObject a HP zero
- Patrol tra N punti
- Inseguimento del giocatore entro range (continua finché il player è nel range)
- Danno al contatto

**Risultato:** primo ciclo di combattimento funzionante.

---

### Giorno 3
**Focus:** sistema essenze e UI base

- Enum dei tipi di essenza
- Essence Manager (gestione quantità)
- Raccolta essenze e modifica quantità
- Drop dai nemici
- UI: visualizzazione quantità essenze

**Risultato:** progressione base tramite risorse presente.

---

### Giorno 4
**Focus:** attacco player e Skill Tree

- Attacco ranged: raycast → collisione → danno al nemico
- `SkillSO` con struttura estendibile per nuove skill
- Sblocco abilità funzionante
- Skill Tree con nodi: `Catalyst Base`, `HP+`, `ATK+`, `Unlock Dash`
- UI: contatore essenze visibile

**Risultato:** loop completo minimo funzionante:

---

### Giorno 5
**Focus:** espansione offensiva, nuova meccanica e polishing

- **Albero offensivo dello Skill Tree:** aggiunti nuovi nodi dedicati al danno e all'attacco
- **Meccanica Pressure:** finché si infligge o si subisce danno, aumentano sia il danno inflitto che quello subito — loop rischio/ricompensa nel combattimento
- **Livello di test:** scena dedicata a validare Skill Tree, Pressure e combattimento in sinergia
- **Polishing Skill Tree:** migliorata UI/UX dell'albero, feedback visivo sullo sblocco delle skill

**Risultato:** sistema di combattimento più profondo, albero espanso, base testabile.

---

### Giorno 6
**Focus:** espansione Skill Tree — rami Movimento (Air) e Cura (Water)

Ramo Movimento:
- **Air Dash:** sblocca la possibilità di eseguire il dash in aria
- **Gale Step:** concede un secondo dash utilizzabile in aria
- **Wind Flow:** riduce il cooldown tra i dash
- **Swift Step:** aumenta la velocità di movimento del 10%

Ramo Cura:
- **Blooming Heals:** alla morte dei nemici possono comparire orb di vita; raccoglierli cura il giocatore
- **Flowing Health:** amplifica l'effetto di ogni cura ricevuta
- **Vitality:** aumenta gli HP massimi del 30%
- **Fluid Body:** l'essenza dell'acqua attutisce le conseguenze di un colpo, estendendo l'invulnerabilità

**Decisione:** le skill dash, double jump, ecc. che servono per la progressione metroidvania verranno sbloccate tramite esplorazione e migliorate dalle skill nel tree.

**Risultato:** Skill Tree ampliato con due nuovi rami tematici (mobilità aerea e sostegno/cura), che si aggiungono al ramo offensivo introdotto al Giorno 5.

---

### Giorno 7
**Focus:** creazione Nemico 2 e Nemico 3

- **Nemico 2 (Crusher):** effettua un dash quando è abbastanza vicino al player
- **Nemico 3 (Archer):** quando in range spara un proiettile; invece di avvicinarsi, si allontana dal player man mano che esso si avvicina
- **Tweak:** aggiunto controllo che evita ai nemici di cadere

**Risultato:** IA nemici più sviluppata e combat più vario.

---

### Giorno 8
**Focus:** UI

- Barra HP animata
- Barra Pressure
- Livello di test
- Hit stop

**Risultato:** UI integrata con i sistemi di gioco e maggiore reattività nel combat.

---

### Giorno 9
**Focus:** Save System

- **Funzione Save:** attivata tramite context menu; salva vita, punto di respawn (non ancora integrato), skill possedute e numero di essenze
- **Funzione Load:** chiamata nello Start del player; carica i dati salvati
- **Funzione ToDefault:** attivata tramite context menu; riporta i dati ai valori default

**Risultato:** sistema di salvataggio base funzionante, in attesa dell'integrazione del punto di respawn.

---

### Giorno 10
**Focus:** implementazione checkpoint

- **Checkpoint:** una volta attivati, settano il punto di respawn e chiamano la funzione `Load()`
- **Da discutere:** funzionamento save, cosa salvare e cosa ricaricare alla morte — argomento di design importante

**Risultato:** si respawna alla morte.

---

### Giorno 11
**Focus:** implementazione finale save system

- **Creazione GDD:** documento contenente le informazioni sul gameplay, da sviluppare quando richiesto dal planning giornaliero
- **Fall death:** quando si cade dalle piattaforme si torna al checkpoint come in caso di morte standard
- **Perdita di essenza:** quando si muore si perde il 20% delle essenze totali e si torna al checkpoint
- **Modifiche attacco:** tramite LineRenderer, quando si attacca viene visualizzato il raggio (da migliorare in futuro)

**Risultato:** polishing di cose già esistenti, nessun sistema nuovo.

---

### Giorno 12
**Focus:** polishing vecchie feature

- **Pressure:** ora la barra fa un flash al cambio tier, aggiunto post-processing della saturazione man mano che aumenta
- **Nemici:** flash bianco quando colpiti
- **Morte:** fade out e fade in quando si muore e si respawna
- **GDD:** sviluppo delle prime aree di gioco e idea di trama a grandi linee

**Risultato:** sistemi rifiniti e bozza di trama.

---

### Giorno 13
**Focus:** tileset e livello di test

- Creazione di un livello di test con tileset e piattaforme

**Risultato:** base ambientale per testare le meccaniche in un contesto più simile al livello finale.

---

### Giorno 14
**Focus:** Dialogue System

- Implementazione sistema di dialogo con textbox e salvataggio dei dialoghi già visti

**Risultato:** base narrativa funzionante e persistente tra le sessioni.

---

### Giorno 15
**Focus:** menu di navigazione

- **Main menu:** all'apertura del gioco si apre il main menu con tasti Play e Quit
- **Esc menu:** premendo Esc si apre un menu che permette di accedere a Skill Tree, Inventario (da fare) e uscire dal gioco

**Risultato:** navigazione base tra le schermate di gioco completata.

---

### Giorno 16
**Focus:** Inventory

- **Inventory:** implementazione sistema di inventario con possibilità di raccogliere oggetti
- **Inventory UI:** creazione di un'interfaccia per visualizzare gli oggetti raccolti e le loro descrizioni

**Domanda aperta:** dubbi di game design — l'inventario serve? Ci devono essere drop dedicati? Potrebbe essere rimosso in futuro.

---

### Giorno 17
**Focus:** Quest System (avvio)

- Implementazione sistema di quest con possibilità di ricevere e completare missioni — ancora WIP

**Risultato:** architettura iniziale in piedi, da completare.

---

### Giorno 18
**Focus:** Quest System (backend)

- Sviluppo backend del sistema quest — UI ancora WIP

**Risultato:** logica di gestione quest pronta, manca la parte visiva.

---

### Giorno 19
**Focus:** Quest System (completamento base)

- Completamento del sistema quest
- Aggiunto un NPC di test
- UI con placeholder finita

**Risultato:** sistema quest utilizzabile end-to-end con dati placeholder.

---

### Giorno 20
**Focus:** Effetti di stato per player e nemici

- **burning:** semplice effetto di danno nel tempo
- **poison:** danno nel tempo e riduzione efficacia cure
- **bleed:** danno nel tempo che aumenta con la velocità di movimento
- **shock:** danno instantaneo e aumento attack cooldown (solo player)
- **weakness:** diminuzione danno inflitto
- **exhaustion:** diminuzione guadagno pressure e aumento perdita pressure (solo player)

**Risultato:** prima versione del sistema di effetti di stato definita, con sei effetti distinti pronti per essere collegati a nemici e meccaniche di combattimento (verrà riorganizzata al Giorno 26).

---

### Giorno 21
**Focus:** Polishing e nuove meccaniche di inventario

- **Save Manager ristrutturato:** riorganizzata l'architettura del sistema di salvataggio per accogliere in modo pulito i nuovi sistemi introdotti nelle settimane precedenti (pozioni in arrivo, quest, effetti di stato)
- **Locked chest:** introdotte casse che richiedono il possesso di uno specifico item nell'inventario per poter essere aperte, prima forma di progressione basata su chiavi
- **Scope dell'inventario:** chiusa la questione aperta dal Giorno 16 — l'inventario sarà dedicato esclusivamente a chiavi e oggetti chiave; per ora nessun drop generico o consumabile passa da lì

**Risultato:** save system più solido e pronto a scalare, aggiunta la prima meccanica di progressione basata su casse bloccate, ruolo dell'inventario finalmente chiarito nel design.

---

### Giorno 22
**Focus:** Sistema pozioni (avvio)

- **Superclasse pozioni curative:** creata una base class comune (`healingPotion` o equivalente) da cui derivano tutte le pozioni di guarigione
- **Heal Potion base:** implementata `basePotion`, la prima pozione concreta derivata dalla superclasse
- **UI elementare:** aggiunta una prima interfaccia grezza per visualizzare la pozione posseduta

**Risultato:** superclasse e prima pozione funzionanti; manca ancora lo swap tra pozioni diverse, una UI rifinita e il binding al tasto di utilizzo.

---

### Giorno 23
**Focus:** Sistema pozioni (input e UI)

- **Binding tasto di uso:** collegato un tasto dedicato all'utilizzo della pozione attualmente attiva
- **UI inventario:** l'inventario ora mostra le pozioni possedute accanto a chiavi e oggetti chiave

**Risultato:** sistema di utilizzo pozioni funzionante end-to-end; mancano ancora lo swap tra pozioni diverse e la relativa meccanica di sblocco.

---

### Giorno 24
**Focus:** Sistema pozioni (completamento) e GDD

- **UI checkpoint:** aggiunta la possibilità di gestire le pozioni direttamente dal menu checkpoint
- **Swap pozioni:** integrata la possibilità di cambiare la pozione attiva tra quelle sbloccate (in previsione delle varianti `emberPotion` e `ragePotion`, ciascuna con un'interazione diversa con la meccanica Pressure)
- **GDD:** migliorato con la descrizione delle varie zone di gioco

**Risultato:** sistema pozioni completo (superclasse, pozione base, UI, binding, swap) e design delle zone più definito nel GDD.

---

### Giorno 25
**Focus:** Design — lista nemici nel GDD

- **Lista nemici:** aggiunta al GDD una lista completa dei nemici, divisa per zona, con relativi attacchi ed eventuali effetti di stato applicati (collegando così il lavoro del Giorno 20 al design delle zone)

**Risultato:** roster nemici completamente pianificato a livello di design, base solida per l'implementazione delle zone successive.

---

### Giorno 26
**Focus:** Rework del sistema effetti di stato e nuovo nemico (Spora)

- **Nemico Spora (avvio):** iniziata l'implementazione di un nuovo nemico che non attacca direttamente ma rilascia una nube tossica quando viene colpito, applicando poison a contatto
- **Rework sistema effetti:** riorganizzata l'architettura introdotta al Giorno 20 in un `statusManager` che ticka ogni frame gli effetti attivi, con sottoclassi dedicate per singolo effetto (a partire da `poison`, che gestisce danno nel tempo e riduzione dei moltiplicatori di cura)

**Risultato:** nemico Spora ancora work in progress; sistema effetti riorganizzato in una struttura più solida ed estendibile, ma non ancora completamente stabile.

---

### Giorno 27
**Focus:** Completamento nemico Spora

- **Spora completata:** finita l'implementazione del nemico, ora completamente funzionante — rilascia correttamente la nube tossica al contatto e applica l'effetto poison tramite il sistema effetti riorganizzato il giorno precedente, riusando il pattern di guardia sulle coroutine (`StopCoroutine` prima di riavviare) già adottato altrove nel progetto

**Risultato:** nemico Spora completamente funzionante e integrato con il rework degli effetti di stato del Giorno 26.

---

### Giorno 28
**Focus:** Costruzione mappa — Foresta Incantata

- **Foresta Incantata:** avviata la costruzione della prima area di gioco vera e propria, usando tileset e piattaforme già preparati al Giorno 13

**Risultato:** prima porzione della zona giocabile, base ambientale su cui costruire level design e incontri.

---

### Giorno 29
**Focus:** Nuovo nemico — Sanguisuga

- **Sanguisuga:** implementato un nuovo nemico che si attacca al player e ruba essenze nel tempo finché rimane agganciato

**Risultato:** nuovo nemico funzionante, aggiunge una minaccia diversa dai pattern già esistenti (contatto diretto, dash, proiettili, nube tossica).

---

### Giorno 30
**Focus:** Fix e polishing

- **Fix Sanguisuga:** corretti bug nel comportamento del nemico introdotto al Giorno 29
- **Fix fastfall:** risolto un problema legato alla discesa rapida del player

**Risultato:** stabilizzati due sistemi esistenti (nemico Sanguisuga e movimento del player).

---

### Giorno 31
**Focus:** Level design — Foresta Incantata

- **Foresta Incantata:** proseguito lo sviluppo del level design della zona, definendo il layout delle piattaforme e il posizionamento di nemici e ostacoli

**Risultato:** level design della zona in progressione, costruito sopra la base ambientale del Giorno 28.

---

### Giorno 32
**Focus:** Level design — Foresta Incantata (continuazione)

- **Foresta Incantata:** ulteriore lavoro sul design della zona, rifinendo struttura e progressione dell'area

**Risultato:** zona Foresta Incantata sempre più definita, avvicinandosi a una prima versione giocabile completa.

---

### Giorno 33

- fine design base forestaIncantata

### Giorno 34

- superclasse boss

### Giorno 35

- inizio boss slime

### Giorno 36

- sviluppo attacchi boss slime

### Giorno 37

- fix boss slime

### Giorno 38

- fix boss slime e building foresta incantata

### Giorno 39

- creazione sistema di transizione tra aree (scene manager)

### Giorno 40

- polishing vari

### Giorno 41

- fix sistema pozioni e UI

### Giorno 42

- inizio rootGolem

### Giorno 43

- attacchi rootGolem

- ### Giorno 44

- fix rootGolem

### Giorno 45

- animazioni spora

### Giorno 46

- animazioni wolf

### Giorno 47

- animazioni hog

### Giorno 48

- fix player

### Giorno 49

- fix save System

### Giorno 50

- rebuilding of ForestaIncatata

### Giorno 51

- riscrittura dialoghi erborista

### Giorno 52

- nuovo gdd ridotto

### Giorno 53

- set up erborista su unity

### Giorno 54

- ristrutturazione quest system e dialogue system

### Giorno 55

- set up quest erborista

### Giorno 56

- ristrutturazione gestione input e scarto inventario non piu necessario nel nuovo scope della demo

### Giorno 57

- ristrtturazione grafica quest menu
## Stato Attuale & Prossimi Passi (fine Giorno 32)

**Sistemi completi e funzionanti:** movimento, combat base, essenze, Skill Tree (rami offensivo/movimento/cura), Save/Load ristrutturato, checkpoint e respawn, morte con fade e perdita essenze, Dialogue System, Main/Esc menu, Inventory (ridotto a chiavi/oggetti chiave, con locked chest), Quest System (backend + UI placeholder + NPC di test), sistema effetti di stato riorganizzato, sistema pozioni completo (superclasse, pozione base, UI, binding, swap), nemici: Crusher, Archer, Spora, Sanguisuga.

**In corso / prossimi step immediati:**
- Completare il level design della Foresta Incantata (avviato al Giorno 31)
- Aggiungere i due nemici di zona ancora mancanti rispetto al GDD: Lupo Corrotto e Cinghiale Contaminato
- Implementare NPC e boss della Foresta Incantata: erborista, slime gigante (main), golem delle radici (secondario), alchimista corrotto (secondario)
- Costruire la sequenza di apertura Prologo/Casa-lab, propedeutica a tutta la trama successiva

**Decisioni aperte:**
- Migliorare visivamente il raggio d'attacco (LineRenderer), nota dal Giorno 11 mai più affrontata.
- Ricompense/potenziamenti ottenibili esplorando la Palude di Grovigli: il GDD la segna come opzionale ma lascia i vantaggi "da stabilire".
- Se e come includere la romance quest tra due NPC — il GDD la segna come eventuale ("solo se ci sta bene").
- Come funziona l'acquisto delle pozioni dall'erborista: il GDD parla sia di pozioni "comprate" sia di pozioni ottenute completando i suoi incarichi — va chiarito se è lo stesso canale o due cose distinte, ed eventualmente con quale valuta/risorsa si comprano.
- Cosa deve succedere perché il Catalizzatore salga al secondo tier dello Skill Tree (il GDD lo lascia come roadmap, senza trigger specifico).
- Dove/come si ottengono nel mondo `doubleJump` e `groundSlam` (il GDD specifica solo che il wall jump è nascosto nel Bosco Profondo).
- Il boss "alchimista corrotto" sbloccherebbe "il dash" — da chiarire se è lo stesso dash già disponibile dal Giorno 1 o un'abilità distinta, per evitare un conflitto di disponibilità.

---

## To-Do List — Tutto Ciò Che Manca per la Demo Completa

Costruita confrontando il GDD con lo stato del progetto a fine Giorno 32. Non include ciò che è già fatto (es. Spora, Sanguisuga d'essenza, sistema pozioni base).

### Sistemi di gioco
- [ ] Secondo tier dello Skill Tree (al momento esiste solo `Catalyst Base`)
- [ ] Abilità metroidvania da esplorazione: `doubleJump`, `wallJump`, `groundSlam` — nessuna delle tre risulta ancora implementata
- [ ] Sistema di acquisto pozioni dall'erborista (valuta/risorsa da definire) + relativa UI negozio
- [ ] Ricarica pozioni ai checkpoint — verificare se già coperta dalla UI checkpoint del Giorno 24 o da implementare esplicitamente
- [ ] Porta del seminterrato a Casa apribile con i due item ottenuti da Torre del Guardiano e Laboratorio Sepolto, per accedere al Nucleo della Corruzione

### Zone / Level Design
- [ ] Prologo / Casa-lab: scena introduttiva completa (boato, seminterrato, cassa con catalizzatore + libro, ricerca e salvataggio della sorella, ritorno a casa)
- [ ] Palude di Grovigli: zona opzionale da costruire, con meccanica di debuff da permanenza in zona
- [ ] Bosco Profondo: zona verticale da costruire (piattaforme mobili, radici che bloccano il passaggio, wall jump nascosto)
- [ ] Torre del Guardiano: zona da costruire
- [ ] Laboratorio Sepolto: zona da costruire, con puzzle e pericoli ambientali
- [ ] Nucleo della Corruzione: zona finale da costruire, stile onirico/staccato

### Nemici mancanti
- [ ] Ricercatore corrotto — Palude di Grovigli
- [ ] Falco delle cime — Bosco Profondo *(il movimento base in volo — patrol/follow — risulta già abbozzato da un lavoro precedente su nemici aerei; da verificare se riutilizzabile)*
- [ ] Ombra — Bosco Profondo
- [ ] Radice Incantata — Bosco Profondo
- [ ] Golem Sentinella — Torre del Guardiano
- [ ] Costrutto metallico — Torre del Guardiano
- [ ] Sbaglio Alchemico "Alfio" — Laboratorio Sepolto
- [ ] Costrutto instabile — Laboratorio Sepolto


### Boss
- [ ] Golem delle radici — Foresta Incantata (secondario)
- [ ] Alchimista corrotto — Foresta Incantata (secondario)
- [ ] Ricercatore fuso con essenze corrotte — Palude di Grovigli (main)
- [ ] Boss immobile del Bosco Profondo (main)
- [ ] Guardiano costrutto — Torre del Guardiano (main)
- [ ] Creazione fallita — Laboratorio Sepolto (main)
- [ ] Mid-boss finale — Nucleo della Corruzione

### NPC e Narrativa
- [ ] Erborista — Foresta Incantata
- [ ] Rampicatore — Bosco Profondo
- [ ] Quest reali collegate agli NPC (finora solo dati placeholder e NPC di test, Giorno 19)
- [ ] Scrittura e integrazione della trama in ogni zona (rivelazioni di lore, dialoghi legati a boss/NPC)
- [ ] Eventuale romance quest tra due NPC (vedi decisioni aperte)
- [ ] Finale della demo (mid boss fight con suspance) al Nucleo della Corruzione

### Comparto Artistico e Audio
- [ ] Art pass per ogni zona coerente con le palette del GDD (casa consumata, foresta viva/corrotta, palude marcia, bosco freddo e nebbioso, torre in rovina, laboratorio con corruzione, nucleo onirico)
- [ ] Sprite e animazioni per tutti i nemici e boss elencati sopra
- [ ] Comparto audio (musiche e SFX) — al momento non risulta ancora avviato