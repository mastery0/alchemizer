# ALCHEMIZER — Diario di Sviluppo

> Aggiornato al termine del **Giorno 20**.

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

### Decisione Aperta (dal Giorno 16)
**L'inventario serve davvero?** Dubbio di game design ancora irrisolto: se sì, servono drop dedicati dai nemici/mondo? Se no, il sistema va rimosso o riconvertito. Da chiudere prima di investire altro tempo nella UI (es. ScrollRect).

---

## Architettura Tecnica

- Motore: **Unity**, linguaggio **C#**
- Le skill usano **ScriptableObject** (`SkillSO`) — struttura estendibile
- Anche il **Quest System** è basato su ScriptableObject
- Attacco ranged basato su **raycast**, visualizzato con **LineRenderer** (da migliorare esteticamente)
- Essenze gestite da un **Essence Manager** centralizzato
- **SaveManager**: singleton con `DontDestroyOnLoad`
- **Core Instability** (pressure): pattern a eventi `OnAttack()` / `OnHit()`, integrato con post-processing URP (vignette, color adjustment, saturazione)
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

- burning: semplice effetto di danno nel tempo
- poison: danno nel tempo e riduzione efficacia cure
- bleed: danno nel tempo che aumenta con la velocità di movimento
- shock: danno instantaneo e aumento attack cd (solo player)
- weakness: diminuzione danno inflitto
- exhaustion: diminuzione guadagno pressure e aumento perdita pressure (solo player)
---

## Stato Attuale & Prossimi Passi (fine Giorno 20)

**Sistemi completi e funzionanti:** movimento, combat base, essenze, Skill Tree (rami offensivo/movimento/cura), 3 tipi di nemico, UI di combattimento (HP bar, Pressure bar, hit stop), Save/Load, checkpoint e respawn, morte con fade e perdita essenze, Dialogue System, Main/Esc menu, Inventory (base + UI), Quest System (backend + UI placeholder + NPC di test).

**In corso / prossimo step immediato:**
- **Inventory:** UI e raccolta funzionano, ma il ruolo di design è ancora incerto (vedi Giorno 16).

**Decisioni aperte:**
- Mantenere o rimuovere l'Inventory? Se mantenuto, servono drop dedicati?
- Migliorare visivamente il raggio d'attacco (LineRenderer), nota dal Giorno 11 mai più affrontata.