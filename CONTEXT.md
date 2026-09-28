# Family Management System

A server and domain model to manage household finances, schedules, and operations, starting with purchase receipt ingestion and archival.

## Language

**Receipt**:
A digital or scanned record of a purchase transaction, consisting of metadata and a referenced file archived in Google Drive.
_Avoid_: Ticket, invoice, bill

**Purchase**:
The exchange of money for goods or services represented by a receipt.
_Avoid_: Transaction, order

**Merchant**:
The business, store, or vendor where a purchase occurred.
_Avoid_: Vendor, seller, shop, retailer

**File Reference**:
A pointer to an archived receipt document in external storage (Google Drive), identified by its unique remote file ID, view link, and file name.
_Avoid_: Attachment, blob, file link

**Storage Provider**:
An external storage service responsible for securely persisting raw receipt files and providing read/download access.
_Avoid_: File server, cloud bucket
