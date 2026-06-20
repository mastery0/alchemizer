# ALCHEMIZER — Diario di Sviluppo

> Aggiornato al termine del **Giorno 9**.

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
Le abilità di movimento si sbloccano tramite **Skill Tree** (ramo Movimento: Air Dash, Gale Step, Wind Flow, Swift Step).

---

## Architettura Tecnica

- Motore: **Unity**, linguaggio **C#**
- Le skill usano **ScriptableObject** (`SkillSO`) — struttura estendibile
- Attacco ranged basato su **raycast** (non proiettile fisico)
- Essenze gestite da un **Essence Manager** centralizzato

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
```
Combattimento → Drop Essenze → Raccolta → Skill Tree → Upgrade statistiche
```

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

**Decisione:** Le skill dash,double jump etc.. che servono per la progressione metroidvania verrano sbloccate tramite esplorazione e migliorate dalle skill nel tree,

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

- **barra hp animata**
- **barra pressure**
- **livello di test**
- **hit stop**

**Risultato:** ui integrata con i sistemi di gioco e piu reattività nel combat

---

### Giorno 9
**Focus:** Save System

- **Funzione Save:** attivata tramite context menu; salva vita, punto di respawn (non ancora integrato), skill possedute e numero di essenze
- **Funzione Load:** chiamata nello Start del player; carica i dati salvati
- **Funzione ToDefault:** attivata tramite context menu; riporta i dati ai valori default

**Risultato:** sistema di salvataggio base funzionante, in attesa dell'integrazione del punto di respawn.