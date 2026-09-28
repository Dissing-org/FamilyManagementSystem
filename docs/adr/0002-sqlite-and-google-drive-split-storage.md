# Split Storage: SQLite for Metadata and Google Drive for Files

Receipts consist of both structured metadata (merchant, purchase date, total amount, currency) and binary files (PDFs, images). We decided to store metadata locally in SQLite via Entity Framework Core for millisecond query and search performance, while streaming and archiving receipt files to Google Drive using a headless Service Account. This minimizes local server disk usage, leverages Google Drive's redundancy and cloud viewers, and avoids tight coupling between the local database and binary file storage.
