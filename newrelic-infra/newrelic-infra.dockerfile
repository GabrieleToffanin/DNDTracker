FROM debian:bookworm-slim AS rabbitmq-integration

ARG NRI_RABBITMQ_VERSION=2.17.3

RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl \
    && curl -fsSL "https://github.com/newrelic/nri-rabbitmq/releases/download/v${NRI_RABBITMQ_VERSION}/nri-rabbitmq_${NRI_RABBITMQ_VERSION}-1_amd64.deb" \
        -o /tmp/nri-rabbitmq.deb \
    && dpkg-deb -x /tmp/nri-rabbitmq.deb /tmp/nri-rabbitmq \
    && rm -rf /var/lib/apt/lists/* /tmp/nri-rabbitmq.deb

FROM debian:bookworm-slim AS postgresql-integration

ARG NRI_POSTGRESQL_VERSION=2.29.0

RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl \
    && curl -fsSL "https://github.com/newrelic/nri-postgresql/releases/download/v${NRI_POSTGRESQL_VERSION}/nri-postgresql_${NRI_POSTGRESQL_VERSION}-1_amd64.deb" \
        -o /tmp/nri-postgresql.deb \
    && dpkg-deb -x /tmp/nri-postgresql.deb /tmp/nri-postgresql \
    && rm -rf /var/lib/apt/lists/* /tmp/nri-postgresql.deb

FROM newrelic/infrastructure:latest

COPY newrelic-infra.yml /etc/newrelic-infra.yml
COPY --from=rabbitmq-integration /tmp/nri-rabbitmq/var/db/newrelic-infra/newrelic-integrations/bin/nri-rabbitmq /var/db/newrelic-infra/newrelic-integrations/bin/nri-rabbitmq
COPY --from=rabbitmq-integration /tmp/nri-rabbitmq/var/db/newrelic-infra/newrelic-integrations/rabbitmq-definition.yml /var/db/newrelic-infra/newrelic-integrations/rabbitmq-definition.yml
COPY rabbitmq-config.yml /etc/newrelic-infra/integrations.d/rabbitmq-config.yml
COPY rabbitmq-log.yml /etc/newrelic-infra/logging.d/rabbitmq-log.yml
COPY --from=postgresql-integration /tmp/nri-postgresql/var/db/newrelic-infra/newrelic-integrations/bin/nri-postgresql /var/db/newrelic-infra/newrelic-integrations/bin/nri-postgresql
COPY --from=postgresql-integration /tmp/nri-postgresql/var/db/newrelic-infra/newrelic-integrations/postgresql-definition.yml /var/db/newrelic-infra/newrelic-integrations/postgresql-definition.yml
COPY postgresql-config.yml /etc/newrelic-infra/integrations.d/postgresql-config.yml
