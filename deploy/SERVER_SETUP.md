# Configuración inicial del servidor Hetzner

Ejecutar una sola vez al preparar el servidor. El server (`portfolio-hel1-1`, `2.29.23.254`) ya está creado y con hardening aplicado (usuario `deploy`, `ufw`, `fail2ban`, swap, Docker) — ver `docs/INFRAESTRUCTURA.md` en la raíz de `D:\Code\projects`. Estos pasos son los específicos de este proyecto.

## 1. Clonar el repositorio

```bash
sudo mkdir -p /opt/kanban && sudo chown deploy:deploy /opt/kanban
git clone https://github.com/tech-marcos-rios/kanban-saas.git /opt/kanban
```

## 2. Crear el archivo de secretos

```bash
cat > /opt/kanban/deploy/.env << 'EOF'
DB_PASSWORD=CAMBIAR_POR_PASSWORD_SEGURO
JWT_KEY=CAMBIAR_POR_CLAVE_MINIMO_32_CARACTERES_ALEATORIA
CORS_ORIGINS=https://TU_FRONTEND.vercel.app
EOF
chmod 600 /opt/kanban/deploy/.env
```

**Nota:** `Cors:AllowedOrigins` se lee separado por comas si hace falta permitir más de un origen (ver `Program.cs`) — importante que SignalR pueda hacer `negotiate` con credenciales, no usar `*`.

## 3. Primer deploy

```bash
cd /opt/kanban
docker compose -f deploy/docker-compose.yml up --build -d
```

## 4. Verificar

```bash
docker compose -f deploy/docker-compose.yml ps
curl http://localhost:5020/health
```

## Secrets requeridos en GitHub Actions

Ir a: Settings → Secrets → Actions → New repository secret

| Secret | Valor |
|--------|-------|
| `HETZNER_HOST` | `2.29.23.254` |
| `HETZNER_USER` | `deploy` |
| `HETZNER_SSH_KEY` | Clave privada SSH dedicada (`p_portfolio_hetzner`, sin passphrase) |

La clave pública correspondiente (`p-portfolio-root`) ya está en `/home/deploy/.ssh/authorized_keys` en el servidor.
