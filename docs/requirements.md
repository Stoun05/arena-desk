# ArenaDesk MVP talaplary

**Dokumentiň ýagdaýy:** 1-nji tapgyr tamamlandy

**Wersiýa:** 1.0

**Maksat:** ilkinji işleýän wersiýanyň hökmany mümkinçiliklerini anyk kesgitlemek

## 1. Proýektiň maksady

ArenaDesk gaming klub we internet-kafe üçin lokal dolandyryş ulgamydyr. Ulgam kassire 10 kompýuteri bir panelden görmäge, tölegli sessiýa açmaga, galan wagty yzarlamaga we sessiýa tamamlananda degişli kompýuteri awtomatik gulplamaga mümkinçilik bermeli.

Ilkinji wersiýanyň esasy gymmaty — sessiýalary, wagt hasabyny we tölegleri ýalňyşsyz dolandyrmak hem-de internet bolmasa-da lokal ulgamda işlemegi dowam etdirmek.

## 2. Maksatlaýyn ulanyjylar

### Kassir

- 10 kompýuteriň häzirki ýagdaýyny görýär;
- boş kompýuterde sessiýa açýar;
- kompýuteriň görnüşine laýyk tarifi saýlaýar;
- sessiýanyň wagtyny uzaldýar ýa-da ir tamamlaýar;
- nagt ýa-da kart tölegini bellige alýar;
- diňe öz işine degişli gündelik maglumatlary görýär.

### Administrator

- kassiriň ähli mümkinçiliklerine eýe;
- kompýuterleri Standard ýa-da VIP görnüşine belleýär;
- tarifleri döredýär we üýtgedýär;
- kassir hasaplaryny döredýär, ýapýar we rollaryny dolandyrýar;
- ähli sessiýalary, tölegleri we audit ýazgylaryny görýär;
- sessiýa gutaranda ýerine ýetiriljek hereketi sazlaýar.

## 3. MVP çägi

### 3.1. Kompýuterler

- Ulgam ilkinji wersiýada takyk 10 kompýuteri dolandyrýar.
- Her kompýuteriň özboluşly belgisi we görkezilýän ady bolýar: mysal üçin, `PC-01`.
- Her kompýuter `Standard` ýa-da `VIP` görnüşine degişli bolýar.
- Standard/VIP sany öňünden berkidilmeýär; administrator islendik kompýuteriň görnüşini saýlap biler.
- Panelde iň az şu ýagdaýlar görkezilýär:
  - `Boş` — täze sessiýa açyp bolýar;
  - `Ulanylýar` — aktiw sessiýa bar;
  - `Öçük/Elýeterli däl` — Windows Agent bilen aragatnaşyk ýok;
  - `Gulplanýar` — sessiýa tamamlanýar we ahyrky hereket ýerine ýetirilýär.
- Aktiw kompýuteriň kartasynda başlan wagty, gutarýan wagty, galan wagt, tarif we töleg ýagdaýy görünýär.

### 3.2. Tarifler we baha hasaby

- Standard we VIP kompýuterler üçin aýratyn sagatlaýyn tarif döredilýär.
- Tarifde at, kompýuter görnüşi, sagatlaýyn baha, walýuta we aktiw/passiw ýagdaý saklanýar.
- Kassir diňe aktiw we saýlanan kompýutere laýyk tarifleri ulanyp biler.
- Sessiýa öňünden töleg esasynda açylýar.
- Baha şu formula bilen hasaplanýar:

  `jemi baha = sagatlaýyn baha × saýlanan minut / 60`

- Pul bahasy iki onluk belgä çenli tegeleklenýär.
- Sessiýa başlanandan soň ulanylan tarif we başlangyç baha taryh üçin üýtgemez görnüşde saklanýar.
- Kassir goşmaça töleg alyp sessiýanyň wagtyny uzaldyp biler.

### 3.3. Sessiýalar

Sessiýa açmak akymy:

1. Kassir `Boş` ýagdaýdaky kompýuteri saýlaýar.
2. Ulgam kompýuteriň görnüşine laýyk tarifleri görkezýär.
3. Kassir dowamlylygy minut ýa-da sagat görnüşinde girizýär.
4. Ulgam gutarýan wagty we jemi bahany awtomatik hasaplaýar.
5. Kassir töleg görnüşini saýlap, sessiýany tassyklaýar.
6. Backend sessiýany ýazga alýar we Windows Agent-e kompýuteri açmak buýrugyny berýär.
7. Panel taýmeri real wagt görnüşinde görkezýär.

Sessiýa üçin hökmany düzgünler:

- bir kompýuterde şol bir wagtda diňe bir aktiw sessiýa bolup biler;
- dowamlylyk 1 minutdan az bolup bilmez;
- kassir wagty uzaldyp ýa-da sessiýany ir tamamlap biler;
- sessiýa gutarmanka 10 minut we 5 minut galanda oýunça duýduryş görkezilýär;
- wagt tamamlananda täze programma açmak petiklenýär we sazlanan ahyrky hereket ýerine ýetirilýär;
- sessiýanyň başlan, uzaldylan we tamamlanan wagtlary taryhda saklanýar;
- kompýuter täzeden açylsa, aktiw sessiýanyň galan wagty dikeldilýär.

### 3.4. Tölegler

- MVP-de iki töleg görnüşi bar: `Nagt` we `Kart`.
- ArenaDesk bank kart maglumatlaryny kabul etmeýär we saklamaýar; diňe tölegiň görnüşini bellige alýar.
- Her töleg sessiýa, kassir, pul möçberi, walýuta we wagt bilen baglanyşdyrylýar.
- Kassir tamamlanan töleg ýazgysyny pozup ýa-da möçberini üýtgedip bilmeýär.
- Ýalňyş tölegi gaýtarmak diňe administrator tarapyndan, sebäbi görkezilip ýerine ýetirilýär.
- Gaýtarylan töleg hem aýratyn audit ýazgysy hökmünde saklanýar.

### 3.5. Sessiýanyň ahyrky hereketi

- Administrator şu hereketleriň birini ulgam boýunça ýa-da aýratyn kompýuter üçin saýlap biler:
  - `Logout` — Windows ulanyjysyndan çykmak;
  - `Sleep` — kompýuteri uky ýagdaýyna geçirmek;
  - `Shutdown` — kompýuteri öçürmek.
- Howpsuz başlangyç sazlama `Logout` bolýar.
- Ahyrky hereket diňe sessiýa tamamlanandan we maglumatlar lokal saklanandan soň ýerine ýetirilýär.
- Windows Agent buýrugy ýerine ýetirip bilmese, panelde ýalňyşlyk görkezilýär we wakanyň ýazgysy saklanýar.

### 3.6. Hasabatlar

- Administrator we kassir häzirki günüň sessiýalaryny görüp biler.
- Administrator saýlanan senä görä ähli sessiýalary we tölegleri görüp biler.
- Gündelik hasabatda iň az şu maglumatlar bolýar:
  - sessiýalaryň sany;
  - Standard we VIP sessiýalarynyň sany;
  - nagt tölegleriň jemi;
  - kart tölegleriniň jemi;
  - gaýtarylan tölegleriň jemi;
  - umumy girdeji.
- Hasabatdaky sanlar diňe ýazga alnan töleglerden hasaplanýar.

## 4. Offline we lokal ulgam talaplary

- ArenaDesk-iň backend-i we maglumat bazasy klubdaky lokal dolandyryjy kompýuterde ýa-da lokal serverde işleýär.
- Web-panel, backend we Windows Agent-lar şol bir lokal ulgam arkaly habarlaşýar.
- Internetiň kesilmegi aktiw sessiýalary, töleg ýazgylaryny ýa-da paneliň lokal işlemegini duruzmaly däl.
- MVP-de işlemek üçin daşarky bulut hyzmatyna hökmany baglylyk bolmaýar.
- Windows Agent backend bilen wagtlaýyn aragatnaşygyny ýitirse:
  - aktiw sessiýanyň taýmerini lokal dowam etdirýär;
  - sessiýa gutaranda öňünden alnan ahyrky hereketi ýerine ýetirýär;
  - wakalary lokal nobatda saklaýar;
  - aragatnaşyk dikeldilende wakalary backend-e iberýär.
- Lokal serveriň özi elýeterli däl bolsa, panel täze sessiýa açmaýar we düşnükli `Server bilen aragatnaşyk ýok` habaryny görkezýär.
- Maglumat bazasynyň awtomatik ätiýaçlyk nusgasy günde azyndan bir gezek döredilýär.

## 5. Howpsuzlyk we audit

- Her kassir we administrator aýratyn login/parol bilen girýär.
- Parollar açyk tekst görnüşinde saklanmaýar.
- Rugsatlar backend tarapyndan rol esasynda barlanýar; diňe interfeýsdäki düwmeleri gizlemek ýeterlik däldir.
- Şu hereketler audit taryhynda saklanýar:
  - login synanyşyklary;
  - sessiýa açmak, uzaltmak we ir tamamlamak;
  - tarif üýtgetmek;
  - kompýuter görnüşini üýtgetmek;
  - tölegi gaýtarmak;
  - ahyrky hereket sazlamasyny üýtgetmek.
- Maliýe we audit ýazgylary adaty ulanyjy tarapyndan pozulmaýar.

## 6. Esasy maglumat obýektleri

- `User` — kassir ýa-da administrator hasaby;
- `Computer` — kompýuteriň belgisi, ady, görnüşi we ýagdaýy;
- `Tariff` — sagatlaýyn baha we degişli kompýuter görnüşi;
- `Session` — kompýuter, tarif, wagtlar, ýagdaý we hasaplanan baha;
- `Payment` — sessiýa, möçber, görnüş, walýuta we kassir;
- `AgentEvent` — Windows Agent-den gelen ýagdaý we buýruk netijesi;
- `AuditLog` — möhüm ulanyjy hereketleriniň üýtgemeýän taryhy;
- `SystemSetting` — ahyrky hereket, duýduryş wagtlary we beýleki sazlamalar.

## 7. Kabul ediş şertleri

MVP şu synaglaryň hemmesinden geçende talaplara laýyk hasaplanýar:

1. Kassir ulgama girip, 10 kompýuteriň ýagdaýyny bir ekranda görýär.
2. Administrator islendik kompýuteri Standard ýa-da VIP görnüşine geçirip bilýär.
3. Kassir boş kompýuter üçin dowamlylyk saýlaýar we ulgam dogry bahany hasaplaýar.
4. Töleg ýazga alnandan soň sessiýa başlanýar we taýmer panelde azalýar.
5. Şol bir kompýuterde ikinji aktiw sessiýa açmak petiklenýär.
6. Kassir goşmaça töleg bilen wagty uzaldyp bilýär.
7. Oýunçy 10 we 5 minut galanda duýduryş alýar.
8. Wagt tamamlananda sazlanan `Logout`, `Sleep` ýa-da `Shutdown` hereketi ýerine ýetirilýär.
9. Internet kesilende aktiw sessiýanyň taýmeri dowam edýär we maglumat ýitmeýär.
10. Agent aragatnaşygy dikeldilende lokal wakalar backend-e geçirilýär.
11. Kassir tarifleri ýa-da ulanyjy rollaryny üýtgedip bilmeýär.
12. Administrator senä görä sessiýa we töleg hasabatyny görüp bilýär.
13. Tölegi gaýtarmak sebäbi bilen audit taryhynda görünýär.
14. Kompýuter täzeden açylandan soň aktiw sessiýanyň galan wagty dikeldilýär.

## 8. MVP-den daşarda

Şu mümkinçilikler ilkinji wersiýa girmeýär:

- müşderiniň özbaşdak online bron etmegi;
- doly müşderi agzalyk we bonus ulgamy;
- iýmit, içgi ýa-da beýleki harytlaryň ammary we satuwy;
- oýun katalogyny uzakdan gurmak we täzelemek;
- mobil programma;
- birnäçe filialy bir ulgamdan dolandyrmak;
- online töleg gateway integrasiýasy;
- bulut analitikasy;
- awtomatik marketing habarlary.

Bu mümkinçilikler MVP durnukly işläninden soň aýratyn tapgyrlarda meýilleşdiriler.

## 9. 1-nji tapgyryň gutaryş şerti

Şu dokument ArenaDesk-iň ilkinji wersiýasynyň hökmany çägini, ulanyjy rollaryny, sessiýa akymyny, töleg düzgünlerini, offline işleýşini we kabul ediş synaglaryny kesgitleýär. Indiki tapgyrda proýektiň tehniki gurluşy we maglumat akymy şu talaplara görä tassyklanar.
